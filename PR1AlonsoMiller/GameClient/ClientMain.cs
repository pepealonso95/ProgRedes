using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;

namespace GameClient
{
    public class ClientMain
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("installConfig.json", optional: true, reloadOnChange: true)
            .Build();
            TcpClient client = new TcpClient();
            Console.WriteLine("Connecting");
            while (!client.Connected)
            {
                client.Connect(IPAddress.Parse(config.GetSection("serverip").Value), 2000);
            }
            Console.WriteLine("Connected");
            new Client().Start(client);
        }

    }
}
