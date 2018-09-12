using System;
using System.Collections.Generic;
using System.Text;
using GameClient.Commands;
using GameComm;

namespace GameClient
{
    public class Interpreter
    {
        public Command InterpretRequest(string command)
        {
            Command interpretation;
            if (command.Equals(TextCommands.REGISTER, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandRegister();

            }
            else if (command.Equals(TextCommands.LOGIN, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandLogin();

            }
            else if (command.Equals(TextCommands.EXIT, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandExit();

            }
            else
            {
                interpretation = new CommandUnknown();
            }
            return interpretation;
        }
        
    }
}
