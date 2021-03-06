﻿using GameComm;
using System;
using System.Text;

namespace GameClient.Commands
{
    public class CommandMove:Command
    {

        public CommandMove() : base(RequestCmd.MOVE)
        {
        }
        public override byte[] Run()
        {
            string header =  CmdReqList.MOVECHARACTER;
            PrintAvailableDirections();
            string move = "";
            while (move.Equals(""))
            {
                Console.WriteLine("Enter first direction:");
                move+= EnterValidDirection();
                Console.WriteLine("Enter second direction:");
                move += EnterValidDirection();
                if (move.Equals(""))
                {
                    Console.WriteLine("Both directions cant be empty");
                }
            }
            int length = System.Text.Encoding.UTF8.GetByteCount(move);
            string strLength = length.ToString().PadLeft(5, '0');
            return Encoding.UTF8.GetBytes(header + strLength + move);
        }

        private string EnterValidDirection()
        {
            string direction = EnterValidLengthString();
            string translatedDir = TransalteDirection(direction);
            while (translatedDir.Equals("INVALID"))
            {
                Console.WriteLine("Invalid Direction, please select from list:");
                PrintAvailableDirections();
                direction = EnterValidLengthString();
                translatedDir = TransalteDirection(direction);
            }
            return translatedDir;
        }

        private string TransalteDirection(string direction)
        {
            string dir = "INVALID";
            if(direction.Equals(TextCommands.MOVEUP, StringComparison.InvariantCultureIgnoreCase))
            {
                dir = "U";
            } else if (direction.Equals(TextCommands.MOVEDOWN, StringComparison.InvariantCultureIgnoreCase))
            {
                dir = "D";
            }
            else if (direction.Equals(TextCommands.MOVELEFT, StringComparison.InvariantCultureIgnoreCase))
            {
                dir = "L";
            }
            else if (direction.Equals(TextCommands.MOVERIGHT, StringComparison.InvariantCultureIgnoreCase))
            {
                dir = "R";
            }else if (direction.Equals(""))
            {
                dir = "";
            }
            return dir;
        }

        private static void PrintAvailableDirections()
        {
            Console.WriteLine("Available Directions:");
            Console.WriteLine(TextCommands.MOVEUP);
            Console.WriteLine(TextCommands.MOVEDOWN);
            Console.WriteLine(TextCommands.MOVELEFT);
            Console.WriteLine(TextCommands.MOVERIGHT);
        }

    }
}
