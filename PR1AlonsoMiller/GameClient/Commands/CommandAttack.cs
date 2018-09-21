using GameComm;
using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandAttack:Command
    {

        public CommandAttack() : base(RequestCmd.ATTACK)
        {
        }
        public override string Run()
        {

            string cmd = CmdReqList.HEADER + CmdReqList.ATTACKCHARACTER+CmdReqList.NO_VAR;
            return cmd;
        }
        

    }
}
