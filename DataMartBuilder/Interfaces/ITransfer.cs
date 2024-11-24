using DataMartBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Interfaces
{
    public interface ITransfer
    {
        IDatabaseConnection TargetDb { get; set; }
        IDatabaseConnection CurrentDb {  get; set; }
        List<Table> Tables { get; set; }
        bool IsUpdate { get; set; }

        public Task<string> TransferDate();
    }
}
