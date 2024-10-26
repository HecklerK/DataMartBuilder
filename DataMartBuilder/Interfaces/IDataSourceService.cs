using System.Data;


namespace DataMartBuilder.Interfaces
{
    public interface IDataSourceService
    {
        Task<List<string>> GetTablesAsync();
        Task<DataTable> FetchTableDataAsync(string tableName, List<string> columns);
    }
}
