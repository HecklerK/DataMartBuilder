using DataMartBuilder.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataMartBuilder.Services
{
    public class SqlServerConnector : IDatabaseConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        private SqlConnection _sqlConnection;

        public SqlServerConnector(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public bool Connect()
        {
            try
            {
                _sqlConnection = new SqlConnection(ConnectionString);
                _sqlConnection.Open();
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
                _sqlConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
