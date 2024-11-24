using DataMartBuilder.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Xml.Serialization;

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

        public string GetData()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    var serverConnection = new ServerConnection(sqlConnection);
                    var server = new Server(serverConnection);

                    var db = server.Databases[sqlConnection.Database];

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
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        [XmlInclude(typeof(SqlServerConnector))]
        public class SerializableContainer
        {
            public IDatabaseConnection databaseConnection { get; set; }
        }
    }
}
