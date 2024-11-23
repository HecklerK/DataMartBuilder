using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Interfaces
{
    public interface ITransfer
    {
        List<IDatabaseConnection> TargetDbs { get; set; }
        IDatabaseConnection CurrentDb {  get; set; }

        public string TransferDate();
    }
}
