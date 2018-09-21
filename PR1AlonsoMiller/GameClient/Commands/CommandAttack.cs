using GameComm;

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
