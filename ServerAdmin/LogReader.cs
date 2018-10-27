using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using GameComm;

namespace ServerAdmin
{
    public class LogReader
    {
        private static Stack<LogEntry> entries;
        public static void Main(string[] args)
        {
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
                    Message message = messageQueue.Receive();
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
    }
}
