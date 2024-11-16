using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace DataMartBuilder.Models
{
    public class DbConnection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public List<Table> DbTables { get; set; }
        [JsonInclude]
        public ConnectionTypes ConnectionType { get; set; }

        public DbConnection(string name, string connectionString, ConnectionTypes connectionType)
        {
            Name = name;
            ConnectionString = connectionString;
            DbTables = new List<Table>();
            ConnectionType = connectionType;
        }
    }
}
