using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMartBuilder.Interfaces
{
    public interface IDatabaseConnection
    {
        string Name { get; set; }
        string ConnectionString { get; set; }
        bool Connect();
        bool Disconnect();
        bool TestConnection();
    }
}
