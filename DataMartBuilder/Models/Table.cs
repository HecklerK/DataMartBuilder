using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Models
{
    public class Table
    {
        string Name {  get; set; }
        List<string> Columns { get; set; }

        public Table(string name, List<string> columns) 
        {
            this.Name = name;
            this.Columns = columns;
        }
    }
}
