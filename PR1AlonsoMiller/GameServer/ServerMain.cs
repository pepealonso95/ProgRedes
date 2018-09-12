using GameComm;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class ServerMain
    {
        public static int count;
        public static void Main(string[] args)
        {
            IPEndPoint ipthis = new IPEndPoint(IPAddress.Parse(CmdReqList.SERVERIP), 2000);
            TcpListener server = new TcpListener(ipthis);
            //pasar a constante
            server.Start(32);
            Console.WriteLine("Listening");
            int count = 0;
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("ClientConnected");
                count++;
                //pasar a constante
                if (count > 100)
                {
                    client.Close();
                    count++;
                }
                Thread tClient = new Thread(() => HandleClient(client));
                tClient.Start();
            }
        }

        public static void HandleClient(TcpClient client)
        {
            ClientHandler handler = new ClientHandler(client);
            handler.Start();
        }
    }
}
