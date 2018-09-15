using GameComm;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace GameServer
{
    public static class ServerMain
    {
        private static readonly object useLock = new object();
        public static bool isConnected;
        public static ICollection<TcpClient> clients;
        public static int count;
        public const int MAX_PLAYERS_CONNECTED = 100;
        public static void Main(string[] args)
        {
            IPEndPoint ipthis = new IPEndPoint(IPAddress.Parse(CmdReqList.SERVERIP), 2000);
            TcpListener server = new TcpListener(ipthis);
            Match.Instance();
            clients = new List<TcpClient>();
            server.Start(Match.MAX_ACTIVE_PLAYERS);
            isConnected = true;
            Console.WriteLine("Listening");
            int count = 0;
            Thread sClient = new Thread(HandleServer);
            sClient.Start();
            while (isConnected)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("ClientConnected");
                clients.Add(client);
                if (count > MAX_PLAYERS_CONNECTED)
                {
                    client.Close();
                    clients.Remove(client);
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

        public static void HandleServer()
        {
            ServerController controller = new ServerController();
            controller.Start();
            isConnected = false;
            CloseAllConnections();
        }

        private static void CloseAllConnections()
        {
            Monitor.Enter(useLock);
            foreach (TcpClient client in clients)
            {
                client.Close();
            }
            Monitor.Exit(useLock);
        }

        public static void BroadcastMessage(string message)
        {
            string header = CmdResList.HEADER + CmdResList.BROADCAST;
            int length = System.Text.Encoding.UTF8.GetByteCount(message);
            string strLength = length.ToString().PadLeft(4, '0');
            string broadcast = header + strLength + message;
            byte[] buffer = Encoding.UTF8.GetBytes(broadcast);
            Monitor.Enter(useLock);
            foreach(TcpClient client in clients)
            {
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
            Monitor.Exit(useLock);
        }
    }
}
