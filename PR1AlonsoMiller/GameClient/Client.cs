using System;
using System.Collections.Generic;
using System.Text;
using GameComm;
using System.Net;
using System.Net.Sockets;
using GameClient.Commands;

namespace GameClient
{
    public class Client
    {
        public bool isConnected;
        private TcpClient socket;
        public Interpreter interpreter;

        public void Start(TcpClient client)
        {
            //tirar otro while para que el programa no se cierre si es necesario
            interpreter = new Interpreter();
            socket = client;
            isConnected = true;
            while (isConnected)
            {
                HandleRequest();
                HandleResponse();
            }
        }
   
        private void HandleRequest()
        {
            if (socket.GetStream().CanWrite)
            {
                string command = "";
                while (command == "" || command == "Error")
                {
                    Console.WriteLine("Enter Command:");
                Command interpretation = interpreter.InterpretRequest(Console.ReadLine());
                 command = interpretation.Run();
                }
                byte[] buffer = Encoding.UTF8.GetBytes(command);
                socket.GetStream().Write(buffer, 0, buffer.Length);

            }
        }

        private void HandleResponse()
        {
            
            if (socket.GetStream().CanRead) {
                byte[] buffer = new byte[ResponseCmd.FIXED_LENGTH];
                RecieveStream(buffer);
                string strBuffer = Encoding.UTF8.GetString(buffer);
                string header = strBuffer.Substring(0, 3);
                if (header.Equals("RES"))
                {
                    if(strBuffer.Substring(3, 2) == "00")
                    {
                        Console.WriteLine("Operation Succesful");
                        return;
                    }
                    else if(strBuffer.Substring(3, 2) == "02")
                    {
                        Console.WriteLine("Login Invalid, Nickname does not match an Registered Player");
                        return;
                    }
                    else if (strBuffer.Substring(3, 2) == "99")
                    {
                        Console.WriteLine("Disconnected from server");
                        isConnected = false;
                        return;
                    }
                }
                string strVarLength = strBuffer.Substring(6, 4);
                int length = Int32.Parse(strVarLength);
                byte[] data = new byte[length];
                RecieveStream(data);
            }
            if (!isConnected)
            {
                socket.GetStream().Close();
                socket.Close();
                return;
            }
        }


        public void Request()
        {
            Console.WriteLine("Enter Command:");
            string enteredValue = Console.ReadLine();
            Command command = interpreter.InterpretRequest(enteredValue);
            command.Run();
        }


        private void RecieveStream(byte[] buffer)
        {
            var recieved = 0;
            while (recieved < buffer.Length)
            {
                var pos = socket.GetStream().Read(buffer, 0, buffer.Length);
                if (pos == 0)
                {
                    throw new SocketException();
                }
                recieved += pos;
            }
        }

    }
}
