using GameComm;
using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandJoin:Command
    {

        public CommandJoin() : base(RequestCmd.JOINMATCH)
        {
        }
        public override string Run()
        {

            string cmd = CmdReqList.HEADER + CmdReqList.JOINMATCH+CmdReqList.NO_VAR;
            return cmd;
        }
        

    }
}
