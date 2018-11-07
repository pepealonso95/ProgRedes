using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAdministrator
{
    class ClientAdmin
    {
        static void Main(string[] args)
        {
            PlayerService.RemoteAdminClient wcf = new PlayerService.RemoteAdminClient();
            PrintAllCommands();
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Enter Command:");
                string cmd = Console.ReadLine();
                if (cmd.Equals("log"))
                {
                    ICollection<string> log = wcf.GetLog();
                    foreach (string item in log)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("ranking"))
                {
                    ICollection<string> ranking = wcf.GetRanking();
                    foreach (string item in ranking)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("stats"))
                {
                    ICollection<string> stats = wcf.GetStatistics();
                    foreach (string item in stats)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("add"))
                {
                    Console.WriteLine("Enter new players nickname");
                    string username = Console.ReadLine();
                    Console.WriteLine(wcf.AddPlayer(username));
                }
                else if (cmd.Equals("modify"))
                {
                    Console.WriteLine("Enter old player nickname");
                    string username = Console.ReadLine();
                    Console.WriteLine("Enter new player nickname");
                    string newUsername = Console.ReadLine();
                    Console.WriteLine(wcf.ModifyPlayer(username, newUsername));
                }
                else if (cmd.Equals("delete"))
                {
                    Console.WriteLine("Enter nickname of player to delete");
                    string username = Console.ReadLine();
                    Console.WriteLine(wcf.DeletePlayer(username));
                }
                else if (cmd.Equals("exit"))
                {
                    exit = true;
                }
                else
                {
                    Console.WriteLine("Invalid command, please try again");
                }
            }
        }


        private static void PrintAllCommands()
        {
            Console.WriteLine("Available Commands:");
            Console.WriteLine("log");
            Console.WriteLine("ranking");
            Console.WriteLine("stats");
            Console.WriteLine("add");
            Console.WriteLine("modify");
            Console.WriteLine("delete");
            Console.WriteLine("exit");
        }

    }
}
