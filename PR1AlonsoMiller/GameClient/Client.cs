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
        public void Start(TcpClient client)
        {
            interpreter = new ClientInterpreter();
            socket = client;
            isConnected = true;
            stream = socket.GetStream();
            Thread rT = new Thread(HandleResponse);
            rT.Start();
            while (isConnected)
            {
                HandleRequest();
            }
        }
   
        private void HandleRequest()
        {
            if (stream.CanWrite)
            {
                string command = "";
                while (command == "" || command == "Error")
                {
                    command = Request();
                }
                if (command == "Exit")
                {
                    stream.Close();
                    isConnected = false;
                }
                else{
                    byte[] buffer = Encoding.UTF8.GetBytes(command);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private void HandleResponse()
        {
            
            while (stream.CanRead&&isConnected)
            {
                byte[] buffer = new byte[CmdResList.FIXED_LENGTH];
                RecieveStream(buffer);
                string strBuffer = Encoding.UTF8.GetString(buffer);
                string header = strBuffer.Substring(0, 3);
                if (header.Equals("RES"))
                {
                    if (strBuffer.Substring(3, 2) == "98")
                    {
                        PrintServerResponse(strBuffer);
                        HandleResponse();
                        return;
                    }
                    else if (strBuffer.Substring(3, 2) == "00")
                    {
                        Console.WriteLine("Operation Succesful");
                        return;
                    }
                    else if (strBuffer.Substring(3, 2) == "01")
                    {
                        Console.WriteLine("Register Invalid, Nickname already exists");
                    }
                    else if (strBuffer.Substring(3, 2) == "02")
                    {
                        Console.WriteLine("Login Invalid, Nickname does not match an Registered Player");
                        return;
                    }
                    
                    else if (strBuffer.Substring(3, 2) == "99")
                    {
                        Console.WriteLine("Disconnected from server, press enter to close");
                        Console.ReadLine();
                        isConnected = false;
                        stream.Close();
                        return;
                    }
                }
                if (isConnected)
                {
                    PrintServerResponse(strBuffer);
                }
                else
                {
                    stream.Close();
                    socket.Close();
                    return;
                }
            }
        }

        private void PrintServerResponse(string strBuffer)
        {
            string strVarLength = strBuffer.Substring(5, 4);
            int length = Int32.Parse(strVarLength);
            byte[] data = new byte[length];
            RecieveStream(data);
            string response = Encoding.UTF8.GetString(data);
            Console.WriteLine(response);
        }

        public string Request()
        {
            Console.WriteLine("Enter Command:");
            string enteredValue = Console.ReadLine();
            Command command = interpreter.InterpretRequest(enteredValue);
            return command.Run();
        }


        private void RecieveStream(byte[] buffer)
        {
            var recieved = 0;
            while (recieved < buffer.Length && isConnected)
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
                catch (SocketException se)
                {
                    if (se.ErrorCode == 10004)
                    {
                        Console.WriteLine("Disconnected from server, exit to close");
                        socket.Close();
                        isConnected = false;
                    }
                }
            }
        }

    }
}
