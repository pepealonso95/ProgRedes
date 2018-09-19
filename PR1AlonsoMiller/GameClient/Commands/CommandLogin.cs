using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameClient.Commands
{
    public class CommandLogin:Command
    {

        public CommandLogin() : base(RequestCmd.LOGIN)
        {
        }
        public override string Run()
        {
            string header = CmdReqList.HEADER + CmdReqList.LOGIN;
            Console.WriteLine("Enter Player Nickname:");
            string nickname = EnterValidLengthString();
            int length = System.Text.Encoding.UTF8.GetByteCount(nickname);
            string strLength = length.ToString().PadLeft(4, '0');
            return header+strLength + nickname;
        }
        

    }
}
