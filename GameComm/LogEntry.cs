using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameComm
{
    [Serializable()]
    public class LogEntry
    {
        public string User { get; set; }
        public string Action { get; set; }
    }
}
