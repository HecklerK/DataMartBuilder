using DataMartBuilder.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataMartBuilder.Services
{
    public class DataMartService : IDataMartService
    {
        private readonly string _destinationConnectionString;

        public DataMartService(string destinationConnectionString)
        {
            _destinationConnectionString = destinationConnectionString;
        }

        public async Task CreateOrUpdateTableAsync(string tableName, List<string> columns, DataTable data)
        {
            var context = new DbContextOptionsBuilder<DbContext>()
                .UseSqlServer(_destinationConnectionString)
                .Options;

            var dbContext = new DbContext(context);

            // Создание таблицы и вставка данных
        }
    }
}
