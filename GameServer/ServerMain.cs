using GameComm;
using System;
using System.Linq;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GameServer
{
    public static class ServerMain
    {
        private static readonly object useLock = new object();
        public static bool isConnected;
        public static ICollection<TcpClient> clients;
        public static int count;
        public const int MAX_PLAYERS_CONNECTED = 100;
        private static TcpListener server;
        public static void Main(string[] args)
        {
            IPEndPoint ipthis = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["serverip"]), Int32.Parse(ConfigurationManager.AppSettings["serverport"]));
            server = new TcpListener(ipthis);
            Match.Instance();
            Match.duration = ConfigurationManager.AppSettings["matchduration"];
            clients = new List<TcpClient>();
            server.Start(Match.MAX_ACTIVE_PLAYERS);
            isConnected = true;
            Console.WriteLine("Listening");
            Thread sClient = new Thread(HandleServer);
            sClient.Start();
            while (isConnected)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    Monitor.Enter(useLock);
                    clients.Add(client);
                    if (clients.Count > MAX_PLAYERS_CONNECTED)
                    {
                        string response =  CmdResList.SERVERFULL + CmdResList.NO_VAR;
                        byte[] send = Encoding.UTF8.GetBytes(response);
                        client.GetStream().Write(send, 0, CmdResList.FIXED_LENGTH);
                        client.GetStream().Close();
                        client.Close();
                        clients.Remove(client);
                    }
                    Monitor.Exit(useLock);
                    Thread tClient = new Thread(() => HandleClient(client));
                    tClient.Start();
                } catch (SocketException se)
                {
                    if (se.ErrorCode == 10004)
                    {
                        isConnected = false;
                        server.Stop();
                        CloseAllConnections();
                    }
                }
            }
        }

        public static void HandleClient(TcpClient client)
        {
            ClientHandler handler = new ClientHandler(client);
            handler.Start();
            Monitor.Enter(useLock);
            clients.Remove(client);
            Monitor.Exit(useLock);
        }

        public static void HandleServer()
        {
            ServerController controller = new ServerController();
            controller.Start();
            server.Stop();
            CloseAllConnections();
            isConnected = false;
        }

        private static void CloseAllConnections()
        {
            Monitor.Enter(useLock);
            while (clients.Count>0)
            {
                TcpClient client = clients.First<TcpClient>();
                ExitConnection(client);
                client.Close();
                clients.Remove(client);
            }
            Monitor.Exit(useLock);
        }

        private static void ExitConnection(TcpClient client)
        {
            string header = CmdResList.EXIT+CmdResList.NO_VAR;
            byte[] buffer = Encoding.UTF8.GetBytes(header);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }

        public static void BroadcastMessage(string message)
        {
            string header = CmdResList.BROADCAST;
            int length = System.Text.Encoding.UTF8.GetByteCount(message);
            string strLength = length.ToString().PadLeft(5, '0');
            string broadcast = header + strLength + message;
            byte[] buffer = Encoding.UTF8.GetBytes(broadcast);
            Monitor.Enter(useLock);
            foreach (TcpClient client in clients)
            {
                while (client.GetStream().DataAvailable)
                {
                }
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
            Monitor.Exit(useLock);
        }
        
    }
}
