using System;
using System.Collections.Generic;
using System.Text;
using GameClient.Commands;
using Domain;
using GameComm;

namespace GameClient
{
    public class ClientInterpreter : IInterpreter
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
            else if (command.Equals(TextCommands.JOINMATCH, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandJoin();

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
            int i = 0;
            Int32.TryParse(command, out i);
            string interpretation = "";
            if (command == CmdResList.BROADCAST) { }
            else if (command == CmdResList.OK)
            {
                interpretation = "Operation Succesful";
            }
            else if (command == CmdResList.REGISTER_INVALID)
            {
                interpretation = "Register Invalid, Nickname already exists";
            }
            else if (command == CmdResList.LOGIN_INVALID)
            {
                interpretation = "Login Invalid, Nickname does not match an Registered Player";
            }
            else if (command == CmdResList.MATCHFINISHED)
            {
                interpretation = "No Match in progress, wait till next start";
            }
            else if (command == CmdResList.NOTLOGGED)
            {
                interpretation = "Must Log in before joining match.";
            }
            else if (command == CmdResList.MATCHFULL)
            {
                interpretation = "Match is full";
            }
            else if (command == CmdResList.INMATCH)
            {
                interpretation = "Player already in match";
            }
            else if (command == CmdResList.EXIT)
            {
                throw new DisconnectedException("Disconnected from server");
            }
            else if (i >= 0 && i <= 99)
            {
                interpretation = "Command not implemented "+command;
            }
            else
            {
                throw new DisconnectedException("Unknown request from server");
            }
            return interpretation;
        }
    }
}
