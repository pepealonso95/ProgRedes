using GameComm;
using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandExit:Command
    {

        public CommandExit() : base(RequestCmd.EXIT)
        {
        }
        public override string Run()
        {
            
            return "Exit";
        }
        

    }
}
