namespace DataMartBuilder.Models
{
    public class DatabaseConnection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public bool IsConnected { get; set; }
    }
}
