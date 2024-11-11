using DataMartBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Models
{
    public class DataMart
    {
        public string Name { get; set; }
        public ObservableCollection<IDatabaseConnection> DatabaseConnections { get; set; }
        public IDatabaseConnection TargetDatabase { get; set; }
        public ObservableCollection<string> SelectedTables { get; set; }
        public List<DataMartLink> Links { get; set; }

        public DataMart()
        {
            DatabaseConnections = new ObservableCollection<IDatabaseConnection>();
            SelectedTables = new ObservableCollection<string>();
        }
    }
}
