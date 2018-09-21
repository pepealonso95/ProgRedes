using GameComm;
using System;

namespace GameClient.Commands
{
    public class CommandSelect:Command
    {

        public CommandSelect() : base(RequestCmd.SELECTCHARACTER)
        {
        }
        public override string Run()
        {
            string header = CmdReqList.HEADER + CmdReqList.SELECTCHARACTER;
            PrintAvailableCharacters();
            Console.WriteLine("Enter Character Selection:");
            string character = EnterValidCharacter();
            int length = System.Text.Encoding.UTF8.GetByteCount(character);
            string strLength = length.ToString().PadLeft(4, '0');
            return header + strLength + character;
        }

        private string EnterValidCharacter()
        {
            string character = EnterValidLengthString();
            
            while (!character.Equals(TextCommands.MONSTER, StringComparison.InvariantCultureIgnoreCase)
                && !character.Equals(TextCommands.SURVIVOR, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Invalid character, please select from list:");
                PrintAvailableCharacters();
                character = EnterValidLengthString();
            }
            return character;
        }

        private static void PrintAvailableCharacters()
        {
            Console.WriteLine("Available Characters:");
            Console.WriteLine(TextCommands.MONSTER);
            Console.WriteLine(TextCommands.SURVIVOR);
        }

    }
}
