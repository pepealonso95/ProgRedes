using System;
using GameComm;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Commands
{
    public class CommandUnknown:Command
    {

        public CommandUnknown() : base(ServerCmd.UNKNOWN)
        {
        }
        public override string Run()
        {
            Console.WriteLine("Invalid Command");
            return "Error";
        }

    }
}
