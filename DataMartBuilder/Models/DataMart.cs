using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Models
{
    public class DataMart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DatabaseConnection> DatabaseConnections { get; set; }
        public string TargetConnectionString { get; set; }
    }
}
