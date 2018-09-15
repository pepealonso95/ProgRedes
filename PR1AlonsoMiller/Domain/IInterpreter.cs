using System;
using Domain;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public interface IInterpreter
    {
        Command InterpretRequest(string command);
    }
}
