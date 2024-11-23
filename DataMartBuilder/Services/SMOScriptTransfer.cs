using DataMartBuilder.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Services
{
    public class SMOScriptTransfer: ITransfer
    {
        public List<IDatabaseConnection> TargetDbs {  get; set; }
        public IDatabaseConnection CurrentDb { get; set; }

        public SMOScriptTransfer(IDatabaseConnection db, List<IDatabaseConnection> targetDb)
        {
            this.CurrentDb = db;
            this.TargetDbs = targetDb;
        }

        public string TransferDate()
        {
            return string.Empty;
        }

        string TransferSpecificTables(string targetConnectionString, string currentConnectionString, List<string> tables)
        {
            var targetConnection = new ServerConnection(new SqlConnection(targetConnectionString));
            var currentConnection = new ServerConnection(new SqlConnection(currentConnectionString));

            var targetServer = new Server(targetConnection);
            var currentServer = new Server(currentConnection);

            string targetDatabaseName = targetServer.ConnectionContext.DatabaseName;
            string currentDatabaseName = currentServer.ConnectionContext.DatabaseName;

            var targetDatabase = targetServer.Databases[targetDatabaseName];
            var currantDatabase = currentServer.Databases[targetDatabaseName];

            if (targetDatabase == null)
                return "Исходная база данных не найдена.";
            if (currantDatabase == null)
                return "Целевая база данных не найдена.";

            var sourceTable = new List<Table>();

            foreach (var table in tables)
            {
                sourceTable.Add(targetDatabase.Tables[table]);
            }
            
            if (!sourceTable.Any())
                return $"Не удалось получить таблицы";

            var transfer = new Transfer(targetDatabase)
            {
                Options = new ScriptingOptions
                {
                    ScriptDrops = false,
                    ScriptSchema = true,
                    ScriptData = true,
                    WithDependencies = false
                },
                CopyData = true,
                CopySchema = true,
                ObjectList = new ArrayList(sourceTable),
                DestinationDatabase = targetDatabaseName,
                DestinationServer = targetServer.ConnectionContext.TrueName,
                DestinationLoginSecure = true
            };

            var scripts = transfer.ScriptTransfer();
            string scriptFilePath = $"./scripts/Transfer_{targetDatabaseName}.sql";

            File.WriteAllText(scriptFilePath, scripts.ToString());

            return ExecuteScriptsOnTarget(scripts, targetConnectionString);
        }

        /// <summary>
        /// Выполняет список SQL-скриптов на целевой базе данных.
        /// </summary>
        /// <param name="scripts">Список SQL-скриптов для выполнения.</param>
        /// <param name="targetConnectionString">Строка подключения к целевой базе данных.</param>
        string ExecuteScriptsOnTarget(StringCollection scripts, string targetConnectionString)
        {
            try
            {
                using (var targetConnection = new SqlConnection(targetConnectionString))
                {
                    targetConnection.Open();
                    foreach (var script in scripts)
                    {
                        using (var command = new SqlCommand(script, targetConnection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
            
            return string.Empty;
        }
    }
}
