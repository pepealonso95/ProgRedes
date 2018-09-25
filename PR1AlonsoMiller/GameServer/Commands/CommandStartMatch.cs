using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Commands
{
    public class CommandStartMatch : Command
    {
        public CommandStartMatch() : base(ServerCmd.STARTMATCH)
        {
        }

        public override byte[] Run()
        {
            Console.WriteLine("Match started, cannot enter commands while match is in progress");
            Match.StartMatch();
            Console.WriteLine("Match Finished");
            return Encoding.UTF8.GetBytes("Ok");
        }
        

    }
}
