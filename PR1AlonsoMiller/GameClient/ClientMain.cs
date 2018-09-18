using System;
using Domain;
using GameComm;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameClient
{
    public class ClientMain
    {
        static int port = 2001;
        static void Main(string[] args)
        {
            
            TcpClient client = new TcpClient();
            Console.WriteLine("Connecting");
            while (!client.Connected)
            {
                client.Connect(IPAddress.Parse(CmdReqList.SERVERIP), 2000);
            }
            Console.WriteLine("Connected");
            new Client().Start(client);
        }

    }
}
