using DataMartBuilder.Interfaces;
using System.Data;

namespace DataMartBuilder.Services
{
    public class SqlDataSourceService : IDataSourceService
    {
        public Task<List<string>> GetTablesAsync() { return null; }
        public Task<DataTable> FetchTableDataAsync(string tableName, List<string> columns) { return null; }
    }
}
