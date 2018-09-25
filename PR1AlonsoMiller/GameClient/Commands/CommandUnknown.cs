using System;
using System.Text;
using GameComm;

namespace GameClient.Commands
{
    public class CommandUnknown:Command
    {

        public CommandUnknown() : base(RequestCmd.UNKNOWN)
        {
        }
        public override byte[] Run()
        {
            Console.WriteLine("Invalid Command");
            return Encoding.UTF8.GetBytes("Error");
        }

    }
}
