using System;
using Domain;
using System.Collections.Generic;
using System.Text;
using GameComm;

namespace GameServer
{
    public class ServerController
    {
        public bool Disconnect;
        public IInterpreter interpreter;

        public void Start()
        {
            PrintCommands();
            interpreter = new ServerInterpreter();
            Disconnect = false;
            while (!Disconnect)
            {
                HandleRequest();
            }
        }
        private void PrintCommands()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine(TextCommands.SHOWREGISTERED);
            Console.WriteLine(TextCommands.SHOWLOGGED);
            Console.WriteLine(TextCommands.STARTMATCH);
            Console.WriteLine(TextCommands.EXIT);
            Console.WriteLine("");
        }

        private void HandleRequest()
        {
            string command = "";
            while (command == "" || command == "Error")
            {
                command = Request();
            }
            if(command == "Exit")
            {
                Disconnect = true;
            }
        }

        public string Request()
        {
            Console.WriteLine("Enter Command:");
            string enteredValue = Console.ReadLine();
            Command command = interpreter.InterpretRequest(enteredValue);
            return command.Run();
        }
    }
}
