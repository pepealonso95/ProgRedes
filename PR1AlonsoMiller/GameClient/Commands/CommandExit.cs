using GameComm;

namespace GameClient.Commands
{
    public class CommandExit:Command
    {

        public CommandExit() : base(RequestCmd.EXIT)
        {
        }
        public override string Run()
        {
            
            return "Exit";
        }
        

    }
}
