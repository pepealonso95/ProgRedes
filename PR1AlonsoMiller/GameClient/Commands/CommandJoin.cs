using GameComm;
using System.Text;

namespace GameClient.Commands
{
    public class CommandJoin:Command
    {

        public CommandJoin() : base(RequestCmd.JOINMATCH)
        {
        }
        public override byte[] Run()
        {

            string cmd = CmdReqList.HEADER + CmdReqList.JOINMATCH+CmdReqList.NO_VAR;
            return Encoding.UTF8.GetBytes(cmd);
        }
        

    }
}
