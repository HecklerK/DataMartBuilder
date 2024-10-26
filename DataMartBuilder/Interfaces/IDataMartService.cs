using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Interfaces
{
    public interface IDataMartService
    {
        Task CreateOrUpdateTableAsync(string tableName, List<string> columns, DataTable data);
    }
}
