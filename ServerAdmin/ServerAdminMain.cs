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
using System.Collections;
using System.Runtime.Serialization.Formatters;

namespace ServerAdmin
{
    public class ServerAdminMain
    {
        private static Stack<LogEntry> entries;
        public static void Main(string[] args)
        {
            CrearInfrastructuraRemoting();
            string destintatario;
            string mensaje;

            Console.WriteLine("Obtener referencia al server.");
            IPlayerList serverChat = (IPlayerList)Activator.GetObject(
                                                    typeof(IPlayerList),
                                                    "tcp://"+ConfigurationManager.AppSettings["serverip"]+":"+ ConfigurationManager.AppSettings["serverport"] + "/ServerChat");

            entries = new Stack<LogEntry>();
            bool exit = false;
            Console.WriteLine("Available Commands:");
            Console.WriteLine("log");
            Console.WriteLine("exit");
            while (!exit)
            {
                Console.WriteLine("Enter Command:");
                string cmd = Console.ReadLine();
                if (cmd.Equals("log"))
                {
                    ReadMessages();
                    ResendMessages();
                }
                else if (cmd.Equals("exit"))
                {
                    exit = true;
                }
            }
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
            props["port"] = 0;
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            new TcpChannel(props, null, serverProvider);
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(chan, false);
        }
    }
}
