using DataMartBuilder.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Services
{
    public class SmoTransfer : ITransfer
    {
        public IDatabaseConnection TargetDb { get; set; }
        public IDatabaseConnection CurrentDb { get; set; }
        public List<Models.Table> Tables { get; set; }
        public bool IsUpdate { get; set; }

        public SmoTransfer(IDatabaseConnection db, IDatabaseConnection targetDb, List<Models.Table> tables, bool isUpdate = false)
        {
            CurrentDb = db;
            TargetDb = targetDb;
            Tables = tables;
            IsUpdate = isUpdate;
        }

        async public Task<string> TransferDate()
        {
            return await TransferSpecificTables(TargetDb.ConnectionString, CurrentDb.ConnectionString, Tables.Select(x => x.Name).ToList());
        }

        public async Task<string> TransferSpecificTables(string targetConnectionString, string currentConnectionString, List<string> tables)
        {
            var targetSqlConnection = new SqlConnection(targetConnectionString);
            var currentSqlConnection = new SqlConnection(currentConnectionString);
            var targetConnection = new ServerConnection(targetSqlConnection);
            var currentConnection = new ServerConnection(currentSqlConnection);

            var targetServer = new Server(targetConnection);
            var currentServer = new Server(currentConnection);

            string targetDatabaseName = targetServer.Databases[targetSqlConnection.Database].Name;
            string currentDatabaseName = currentServer.Databases[currentSqlConnection.Database].Name;

            var targetDatabase = targetServer.Databases[targetDatabaseName];
            var currantDatabase = currentServer.Databases[currentDatabaseName];

            if (targetDatabase == null)
                return "Исходная база данных не найдена.";
            if (currantDatabase == null)
                return "Целевая база данных не найдена.";

            foreach (Table sourceTable in targetDatabase.Tables)
            {
                if (Tables.Any(x => x.Name == sourceTable.Name))
                {
                    TransferTableStructure(sourceTable, currantDatabase);
                    TransferTableData(sourceTable.Name);
                }
            }

            return string.Empty;
        }

        private void TransferTableStructure(Table sourceTable, Database currentDatabase)
        {
            if (currentDatabase.Tables.Contains(sourceTable.Name))
            {
                return;
            }

            var targetTable = new Table(currentDatabase, sourceTable.Name);

            foreach (Column sourceColumn in sourceTable.Columns)
            {
                var targetColumn = new Column(targetTable, sourceColumn.Name, sourceColumn.DataType)
                {
                    Nullable = sourceColumn.Nullable,
                    Identity = sourceColumn.Identity
                };
                targetTable.Columns.Add(targetColumn);
            }

            targetTable.Create();
        }

        /// <summary>
        /// Перенос данных из одной таблицы в другую.
        /// </summary>
        private void TransferTableData(string tableName)
        {
            string selectQuery = $"SELECT * FROM [{tableName}]";

            using (var targetConnection = new SqlConnection(TargetDb.ConnectionString))
            using (var currantConnection = new SqlConnection(CurrentDb.ConnectionString))
            {
                targetConnection.Open();
                currantConnection.Open();

                using (var sourceCommand = new SqlCommand(selectQuery, targetConnection))
                using (var reader = sourceCommand.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    using (var bulkCopy = new SqlBulkCopy(currantConnection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.WriteToServer(dataTable);
                    }
                }
            }
        }
    }
}
