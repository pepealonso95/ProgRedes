using GameComm;

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
