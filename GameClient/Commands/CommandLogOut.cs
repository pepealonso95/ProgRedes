using GameComm;
using System.Text;

namespace GameClient.Commands
{
    public class CommandLogOut:Command
    {

        public CommandLogOut() : base(RequestCmd.LOGOUT)
        {
        }
        public override byte[] Run()
        {
            return Encoding.UTF8.GetBytes( CmdReqList.LOGOUT + CmdReqList.NO_VAR);
        }
        

    }
}
