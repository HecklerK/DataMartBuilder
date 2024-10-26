using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Models
{
    public class DataMartLink
    {
        public string SourceTable { get; set; }
        public string TargetTable { get; set; }
        public List<(string SourceColumn, string TargetColumn)> LinkColumns { get; set; } = new List<(string, string)>();

        public override string ToString()
        {
            return $"{SourceTable} -> {TargetTable}";
        }
    }
}
