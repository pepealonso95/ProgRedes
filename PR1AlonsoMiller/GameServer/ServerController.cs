using System;
using Domain;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class ServerController
    {
        public bool Disconnect;
        public ServerInterpreter interpreter;

        public void Start()
        {
            interpreter = new ServerInterpreter();
            Disconnect = false;
            while (!Disconnect)
            {
                HandleRequest();
            }
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
