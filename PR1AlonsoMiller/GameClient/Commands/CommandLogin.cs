using GameComm;
using System;
using System.Text;

namespace GameClient.Commands
{
    public class CommandLogin:Command
    {

        public CommandLogin() : base(RequestCmd.LOGIN)
        {
        }
        public override byte[] Run()
        {
            string header = CmdReqList.HEADER + CmdReqList.LOGIN;
            Console.WriteLine("Enter Player Nickname:");
            string nickname = EnterValidLengthString();
            int length = Encoding.UTF8.GetByteCount(nickname);
            string strLength = length.ToString().PadLeft(5, '0');
            return Encoding.UTF8.GetBytes(header +strLength + nickname);
        }
        

    }
}
