using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Commands
{
    public class CommandExit:Command
    {

        public CommandExit() : base(ServerCmd.EXIT)
        {
        }
        public override byte[] Run()
        {
            return Encoding.UTF8.GetBytes("Exit");
        }
        

    }
}
