using System;
using System.Collections.Generic;
using System.Text;
using GameComm;
using Domain;
using System.Net;
using System.Net.Sockets;
using GameClient.Commands;
using System.Threading;

namespace GameClient
{
    public class Client
    {
        public bool isConnected;
        private TcpClient socket;
        public ClientInterpreter interpreter;
        private NetworkStream stream;
        private bool expectingResult;
        private static readonly object useLock = new object();
        public void Start(TcpClient client)
        {
            expectingResult = false;
            PrintCommands();
            interpreter = new ClientInterpreter();
            socket = client;
            isConnected = true;
            stream = socket.GetStream();
            Thread resT = new Thread(HandleResponse);
            resT.Start();
            Thread reqT = new Thread(HandleRequest);
            reqT.Start();
        }

        private void PrintCommands()
        {
            Console.WriteLine("");
            Console.WriteLine("Available Commands:");
            Console.WriteLine(TextCommands.REGISTER);
            Console.WriteLine(TextCommands.LOGIN);
            Console.WriteLine(TextCommands.LOGOUT);
            Console.WriteLine(TextCommands.JOINMATCH);
            Console.WriteLine(TextCommands.EXIT);
            Console.WriteLine("");
            Console.WriteLine("Available Commands after Joining match:");
            Console.WriteLine(TextCommands.SELECTCHARACTER);
            Console.WriteLine(TextCommands.MOVE);
            Console.WriteLine(TextCommands.ATTACK);
            Console.WriteLine(TextCommands.LOGOUT);
            Console.WriteLine(TextCommands.EXIT);
            Console.WriteLine("");
        }

        private void HandleRequest()
        {
            try { 
            while (stream.CanWrite&& isConnected)
            {
                string command = "";
                while ((command == "" || command == "Error")&&isConnected)
                {
                    if (command == "Error")
                        PrintCommands();
                    if (!expectingResult)
                        command = Request();
                    }
                if (command == "Exit"||!isConnected)
                {
                    throw new DisconnectedException("Goodbye");
                }
                else
                    {
                    byte[] buffer = Encoding.UTF8.GetBytes(command);
                    stream.Write(buffer, 0, buffer.Length);
                    expectingResult = true;
                    }
                }
            }
            catch (DisconnectedException e)
            {
                if (isConnected)
                {
                    isConnected = false;
                    Console.WriteLine(e.Message + ", press enter to close");
                    Console.ReadLine();
                    stream.Close();
                    socket.Close();
                }
            }
        }

        private void HandleResponse()
        {
            try
            {
                while (stream.CanRead && isConnected)
                {
                    byte[] buffer = new byte[CmdResList.FIXED_LENGTH];
                    RecieveStream(buffer);
                    string strBuffer = Encoding.UTF8.GetString(buffer);
                    string header = strBuffer.Substring(0, 3);
                    if (header.Equals("RES"))
                    {
                        InterpretResponse(strBuffer.Substring(3, 2));
                        if (isConnected)
                            PrintServerResponse(strBuffer);
                    }
                }
            }
            catch (DisconnectedException e)
            {
                if (isConnected)
                {
                    isConnected = false;
                    Console.WriteLine(e.Message + ", press enter to close");
                    stream.Close();
                    socket.Close();
                }
            }
        }

        private void InterpretResponse(string response)
        {
            string interpretation = interpreter.InterpretResponse(response);
            if (interpretation != "")
            {
                Console.WriteLine(interpretation);
            }
        }

        private void PrintServerResponse(string strBuffer)
        {
            if (strBuffer.Length > 8)
            {
                string strVarLength = strBuffer.Substring(5, 4);
                int length = Int32.Parse(strVarLength);
                if (length > 0)
                {
                    byte[] data = new byte[length];
                    RecieveStream(data);
                    string response = Encoding.UTF8.GetString(data);
                    Console.WriteLine(response);
                }
            }
            if (strBuffer.Substring(3, 2) != CmdResList.BROADCAST)
                expectingResult = false;
        }

        public string Request()
        {
            
            Console.WriteLine("Enter Command:");
            string enteredValue = Console.ReadLine();
            Command command = interpreter.InterpretRequest(enteredValue);
            if (isConnected)
            {
                return command.Run();
            }
            return "Exit";
        }


        private void RecieveStream(byte[] buffer)
        {
            var recieved = 0;
            while (recieved < buffer.Length&&isConnected)
            {
                try
                {
                    var pos = stream.Read(buffer, 0, buffer.Length);
                    if (pos == 0)
                    {
                        socket.Close();
                        isConnected = false;
                    }
                    recieved += pos;
                }
                catch (System.IO.IOException se)
                {
                    throw new DisconnectedException("Lost Connection");
                }
            }
        }

    }
}
