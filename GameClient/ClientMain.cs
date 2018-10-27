using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;

namespace GameClient
{
    public class ClientMain
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            Console.WriteLine("Connecting");
            while (!client.Connected)
            {
                client.Connect(IPAddress.Parse(ConfigurationManager.AppSettings["serverip"]), Int32.Parse(ConfigurationManager.AppSettings["serverport"]));
            }
            Console.WriteLine("Connected");
            new Client().Start(client);
        }

    }
}
