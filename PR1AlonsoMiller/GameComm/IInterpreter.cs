using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public interface IInterpreter
    {
        Command InterpretRequest(string command);
        string InterpretResponse(string command);
    }
}
