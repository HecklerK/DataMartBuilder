using DataMartBuilder.Interfaces;
using System.Collections.ObjectModel;

namespace DataMartBuilder.Models
{
    public class DataMart
    {
        public string Name { get; set; }
        public ObservableCollection<DbConnection> DatabaseConnections { get; set; }
        public DbConnection TargetDatabase { get; set; }
        public ObservableCollection<string> SelectedTables { get; set; }
        public List<string> Links { get; set; }

        public DataMart()
        {
            DatabaseConnections = new ObservableCollection<DbConnection>();
            SelectedTables = new ObservableCollection<string>();
        }
    }
}
