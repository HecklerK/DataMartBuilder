using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Models
{
    public class SelectedTable
    {
        public string TableName { get; set; }
        public Table Table { get; set; }
        public DbConnection Database { get; set; }

        public SelectedTable(Table table, DbConnection database)
        {
            Table = table;
            Database = database;
            TableName = $"{database.Name}.{Table.Name}";
        }
    }
}
