using GameComm;
using System.Text;

namespace GameClient.Commands
{
    public class CommandAttack:Command
    {

        public CommandAttack() : base(RequestCmd.ATTACK)
        {
        }
        public override byte[] Run()
        {

            string cmd = CmdReqList.ATTACKCHARACTER+CmdReqList.NO_VAR;
            return Encoding.UTF8.GetBytes(cmd);
        }
        

    }
}
