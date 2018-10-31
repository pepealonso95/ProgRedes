using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using GameComm;
using Domain;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using System.Configuration;

namespace ServerAdmin
{
    public class ServerAdminMain
    {
        private static Stack<LogEntry> entries;
        private static List<PlayerScore> players = new List<PlayerScore>();
        public static void Main(string[] args)
        {
            CrearInfrastructuraRemoting();

            Console.WriteLine("Obtener referencia al server.");
            IPlayerHandler playerHandler = (IPlayerHandler)Activator.GetObject(
                                                    typeof(IPlayerHandler),
                                                    "tcp://" + ConfigurationManager.AppSettings["serverip"] + ":" + ConfigurationManager.AppSettings["remotingport"] + "/PlayerHandler");
            
            entries = new Stack<LogEntry>();
            bool exit = false;
            PrintAllCommands();
            while (!exit)
            {
                Console.WriteLine("Enter Command:");
                string cmd = Console.ReadLine();
                if (cmd.Equals("log"))
                {
                    ReadMessages();
                    ResendMessages();
                }
                else if (cmd.Equals("ranking"))
                {
                    players =  playerHandler.GetPlayers();
                    PrintRanking();
                }
                else if (cmd.Equals("stats"))
                {
                    players = playerHandler.GetPlayers();
                    PrintStatistics();
                }
                else if (cmd.Equals("exit"))
                {
                    exit = true;
                }
            }
        }

        private static void PrintStatistics()
        {
            List<PlayerScore> scores = players.OrderByDescending(s => s.Match).ToList();
            if (scores.Count == 0)
            {
                Console.WriteLine("No scores");
                return;
            }
            int currentPlayer = 0;
            int initialMatch = scores[currentPlayer].Match;
            while (currentPlayer < scores.Count()&&initialMatch -scores[currentPlayer].Match<10)
            {
                Console.WriteLine("Player: " + scores[currentPlayer].User + ", Role: " + scores[currentPlayer].Role + ", Match: " + scores[currentPlayer].Match+", Won: " + scores[currentPlayer].Survived);
                currentPlayer++;
            }
        }

        private static void PrintRanking()
        {
            List<PlayerScore> scores = players.OrderByDescending(s => s.Score).Take(10).ToList();
            foreach (PlayerScore player in scores)
            {
                Console.WriteLine("Player: "+player.User+", Score: "+player.Score+ ", Role: " + player.Role + ", Date: "+player.Date);
            }
        }

        private static void PrintAllCommands()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("log");
            Console.WriteLine("ranking");
            Console.WriteLine("stats");
            Console.WriteLine("exit");
        }

        private static void ReadMessages()
        {
            bool continueToSeekForMessages = true;

            while (continueToSeekForMessages)
            {
                try
                {
                    var messageQueue = new MessageQueue(@".\private$\matchLog");
                    messageQueue.Formatter = new BinaryMessageFormatter();
                        Message message = messageQueue.Receive(new TimeSpan(0,0,10));
                        LogEntry entry = (LogEntry)message.Body;
                        entries.Push(entry);
                        Console.WriteLine(entry.User + " -> " + entry.Action);
                }
                catch (Exception ex)
                {
                    continueToSeekForMessages = false;
                    string error = ex.Message;
                }
            }
        }

        private static void ResendMessages()
        {
            while (entries.Count>0)
            {
                try
                {
                    var messageQueue = new MessageQueue(@".\private$\matchLog");
                    messageQueue.Formatter = new BinaryMessageFormatter();

                    LogEntry entry = entries.Pop();
                    messageQueue.Send(entry);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }


        private static void CrearInfrastructuraRemoting()
        {
            IDictionary props = new Hashtable();
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            new TcpChannel(props, null, serverProvider);
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(chan, false);
        }
    }
}
