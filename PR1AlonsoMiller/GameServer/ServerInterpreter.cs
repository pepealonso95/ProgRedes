using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using GameComm;
using GameServer.Commands;

namespace GameServer
{
    public class ServerInterpreter : IInterpreter
    {
        public Command InterpretRequest(string command)
        {
            Command interpretation;
            if (command.Equals(TextCommands.SHOWREGISTERED, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandShowRegistered();

            }
            else if (command.Equals(TextCommands.SHOWLOGGED, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandShowLogged();

            }
            else if (command.Equals(TextCommands.STARTMATCH, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandStartMatch();

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

        public string InterpretResponse(string command)
        {
            throw new NotImplementedException();
        }
    }
}
