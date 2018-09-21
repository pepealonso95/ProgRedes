using System;
using GameComm;

namespace GameClient.Commands
{
    public class CommandUnknown:Command
    {

        public CommandUnknown() : base(RequestCmd.UNKNOWN)
        {
        }
        public override string Run()
        {
            Console.WriteLine("Invalid Command");
            return "Error";
        }

    }
}
