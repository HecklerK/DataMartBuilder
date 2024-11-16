using DataMartBuilder.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;

namespace DataMartBuilder.Services
{
    public class SqlServerConnector : IDatabaseConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public List<Models.Table> DbTables { get; set; }
        private SqlConnection _sqlConnection;

        public SqlServerConnector(Models.DbConnection dbConnection)
        {
            Name = dbConnection.Name;
            ConnectionString = dbConnection.ConnectionString;
            DbTables = dbConnection.DbTables;
        }

        public SqlServerConnector(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
            DbTables = new List<Models.Table>();
        }

        public bool Connect()
        {
            try
            {
                _sqlConnection = new SqlConnection(ConnectionString);
                _sqlConnection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                _sqlConnection.CloseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void GetData()
        {
            var serverConnection = new ServerConnection(ConnectionString);
            var server = new Server(serverConnection);

            var db = server.Databases[0];

            foreach (Table table in db.Tables)
            {
                var listColumns = new List<string>();

                foreach (Column column in table.Columns)
                {
                    listColumns.Add(column.Name);
                }

                DbTables.Add(new Models.Table(table.Name, listColumns));
            }
        }

        [XmlInclude(typeof(SqlServerConnector))]
        public class SerializableContainer
        {
            public IDatabaseConnection databaseConnection { get; set; }
        }
    }
}
