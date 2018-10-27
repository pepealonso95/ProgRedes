using GameComm;
using System.Text;

namespace GameClient.Commands
{
    public class CommandExit:Command
    {

        public CommandExit() : base(RequestCmd.EXIT)
        {
        }
        public override byte[] Run()
        {
            
            return Encoding.UTF8.GetBytes("Exit");
        }
        

    }
}
