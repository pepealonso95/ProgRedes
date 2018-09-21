using GameComm;

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
