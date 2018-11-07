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

namespace ServerAdministrator
{
    public class RemoteAdmin : IRemoteAdmin
    {
        private static Stack<LogEntry> entries;
        private static List<PlayerScore> players = new List<PlayerScore>();
        private static IPlayerHandler playerHandler;
        public RemoteAdmin()
        {
            if(playerHandler == null)
            {
                CrearInfrastructuraRemoting();
                playerHandler = (IPlayerHandler)Activator.GetObject(
                                            typeof(IPlayerHandler),
                                            "tcp://" + ConfigurationManager.AppSettings["serverip"] + ":" + ConfigurationManager.AppSettings["remotingport"] + "/PlayerHandler");
                if (entries == null)
                {
                    entries = new Stack<LogEntry>();
                }
            }
        }

        private void CrearInfrastructuraRemoting()
        {
            IDictionary props = new Hashtable();
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            new TcpChannel(props, null, serverProvider);
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(chan, false);
        }

        public ICollection<string> GetStatistics()
        {
            ICollection<string> statistics = new List<string>();
            List<PlayerScore> scores = players.OrderByDescending(s => s.Match).ToList();
            if (scores.Count == 0)
            {
                statistics.Add("No scores");
                return statistics;
            }
            int currentPlayer = 0;
            int initialMatch = scores[currentPlayer].Match;
            while (currentPlayer < scores.Count() && initialMatch - scores[currentPlayer].Match < 10)
            {
                statistics.Add("Player: " + scores[currentPlayer].User + ", Role: " + scores[currentPlayer].Role + ", Match: " + scores[currentPlayer].Match + ", Won: " + scores[currentPlayer].Survived);
                currentPlayer++;
            }
            return statistics;
        }


        public ICollection<string> GetRanking()
        {
            players = playerHandler.GetPlayers();
            List<PlayerScore> scores = players.OrderByDescending(s => s.Score).Take(10).ToList();
            ICollection<string> ranking = new List<string>();
            foreach (PlayerScore player in scores)
            {
               ranking.Add("Player: " + player.User + ", Score: " + player.Score + ", Role: " + player.Role + ", Date: " + player.Date);
            }
            return ranking;
        }

        public ICollection<string> GetLog()
        {
            ICollection<String> log = ReadMessages();
            ResendMessages();
            return log;
        }

        private ICollection<string> ReadMessages()
        {
            bool continueToSeekForMessages = true;
            ICollection<string> log = new List<string>();
            while (continueToSeekForMessages)
            {
                try
                {
                    var messageQueue = new MessageQueue(@".\private$\matchLog");
                    messageQueue.Formatter = new BinaryMessageFormatter();
                    Message message = messageQueue.Receive(new TimeSpan(0, 0, 10));
                    LogEntry entry = (LogEntry)message.Body;
                    entries.Push(entry);
                    log.Add(entry.User + " -> " + entry.Action);
                }
                catch (Exception ex)
                {
                    continueToSeekForMessages = false;
                    log.Add(ex.Message);
                }
            }
            return log;
        }

        private  void ResendMessages()
        {
            while (entries.Count > 0)
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

        public string AddPlayer(string username)
        {
            return playerHandler.AddPlayer(username);
        }

        public string ModifyPlayer(string oldUsername, string newUsername)
        {
            return playerHandler.ModifyPlayer(oldUsername,newUsername);
        }

        public string DeletePlayer(string username)
        {
            return playerHandler.DeletePlayer(username);
        }
    }
}
