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
            else if (command.Equals(TextCommands.LOGOUT, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandLogOut();

            }
            else if (command.Equals(TextCommands.JOINMATCH, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandJoin();

            }
            else if (command.Equals(TextCommands.SELECTCHARACTER, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandSelect();

            }
            else if (command.Equals(TextCommands.MOVE, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandMove();

            }
            else if (command.Equals(TextCommands.ATTACK, StringComparison.InvariantCultureIgnoreCase))
            {
                interpretation = new CommandAttack();

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
                interpretation = "Login Invalid, Nickname in use or doesnt exist";
            }
            else if (command == CmdResList.SERVERFULL)
            {
                interpretation = "Server is full, cannout connect";
            }
            else if (command == CmdResList.ALREADY_LOGGED)
            {
                interpretation = "Must loggout of current user";
            }
            else if (command == CmdResList.MATCHFINISHED)
            {
                interpretation = "No Match in progress, wait till next start";
            }
            else if (command == CmdResList.NOTLOGGED)
            {
                interpretation = "Must be logged to perform this action";
            }
            else if (command == CmdResList.MATCHFULL)
            {
                interpretation = "Match is full";
            }
            else if (command == CmdResList.INMATCH)
            {
                interpretation = "Player already in match";
            }
            else if (command == CmdResList.INVALID_WHILE_PLAYING)
            {
                interpretation = "Invalid action while in match";
            }
            else if (command == CmdResList.NOT_PLAYING)
            {
                interpretation = "Invalid action out of match";
            }
            else if (command == CmdResList.OUT_OF_BOUNDS)
            {
                interpretation = "Cant move out of board limits";
            }
            else if (command == CmdResList.OCCUPIED)
            {
                interpretation = "Position is already occupied by another player";
            }
            else if (command == CmdResList.PLAYERDEAD)
            {
                interpretation = "Your player is dead,";
            }
            else if (command == CmdResList.NOTINMATCH)
            {
                interpretation = "You must be in match to perform this action";
            }
            else if (command == CmdResList.DIDNT_SELECT)
            {
                interpretation = "You must select a character first";
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
