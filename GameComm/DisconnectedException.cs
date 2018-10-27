using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public class DisconnectedException : Exception
    {
        public DisconnectedException(string message) : base(message)
        {
        }
    }
}
