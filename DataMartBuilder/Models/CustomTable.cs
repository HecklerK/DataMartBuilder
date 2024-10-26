namespace DataMartBuilder.Models
{
    public class CustomTable
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public List<CustomColumn> Columns { get; set; }
    }
}
