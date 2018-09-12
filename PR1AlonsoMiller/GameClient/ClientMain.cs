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
        static void Main(string[] args)
        {
            IPEndPoint ipend = new IPEndPoint(IPAddress.Parse(CmdReqList.SERVERIP), 2001);
            TcpClient client = new TcpClient(ipend);
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
