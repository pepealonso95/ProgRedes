using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandLogOut:Command
    {

        public CommandLogOut() : base(RequestCmd.LOGOUT)
        {
        }
        public override string Run()
        {
            return CmdReqList.HEADER + CmdReqList.LOGOUT + CmdReqList.NO_VAR;
        }
        

    }
}
