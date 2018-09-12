using GameComm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandExit:Command
    {

        public CommandExit() : base(RequestCmd.LOGIN)
        {
        }
        public override string Run()
        {
            string header = CmdReqList.HEADER + CmdReqList.EXIT;
            string strLength = CmdReqList.NO_VAR;
            return header+strLength;
        }
        

    }
}
