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
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ServerAdministrator
{
    public class ServerAdminMain
    {
        private static IRemoteAdmin remoteAdmin;
        public static void Main(string[] args)
        {

            Console.WriteLine("Obtener referencia al server.");
            remoteAdmin = new RemoteAdmin();
            StartWCF();
            bool exit = false;
            PrintAllCommands();
            while (!exit)
            {
                Console.WriteLine("Enter Command:");
                string cmd = Console.ReadLine();
                if (cmd.Equals("log"))
                {
                    ICollection<string> log = remoteAdmin.GetLog();
                    foreach (string item in log)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("ranking"))
                {
                    ICollection<string> ranking = remoteAdmin.GetRanking();
                    foreach (string item in ranking)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("stats"))
                {
                    ICollection<string> stats = remoteAdmin.GetStatistics();
                    foreach (string item in stats)
                    {
                        Console.WriteLine(item);
                    }
                }
                else if (cmd.Equals("add"))
                {
                    Console.WriteLine("Enter new players nickname");
                    string username = Console.ReadLine();
                    Console.WriteLine(remoteAdmin.AddPlayer(username));
                }
                else if (cmd.Equals("modify"))
                {
                    Console.WriteLine("Enter old player nickname");
                    string username = Console.ReadLine();
                    Console.WriteLine("Enter new player nickname");
                    string newUsername = Console.ReadLine();
                    Console.WriteLine(remoteAdmin.ModifyPlayer(username,newUsername));
                }
                else if (cmd.Equals("delete"))
                {
                    Console.WriteLine("Enter nickname of player to delete");
                    string username = Console.ReadLine();

                    Console.WriteLine(remoteAdmin.DeletePlayer(username));
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


        private static void StartWCF()
        {
            ServiceHost playersServiceHost = null;
            try
            {


                //Base Address for GameService
                Uri httpBaseAddress = new Uri("http://"+ ConfigurationManager.AppSettings["currentip"] + ":"+ConfigurationManager.AppSettings["wcfport"]+"/GameService");

                //Instantiate ServiceHost
                playersServiceHost = new ServiceHost(
                    typeof(RemoteAdmin),
                    httpBaseAddress);

                //Add Endpoint to Host
                playersServiceHost.AddServiceEndpoint(
                    typeof(IRemoteAdmin),
                                                        new WSHttpBinding(), "");

                //Metadata Exchange
                ServiceMetadataBehavior serviceBehavior =
                    new ServiceMetadataBehavior();
                serviceBehavior.HttpGetEnabled = true;
                playersServiceHost.Description.Behaviors.Add(serviceBehavior);

                //Open
                playersServiceHost.Open();
                Console.WriteLine("Service is live now at: {0}", httpBaseAddress);
            }
            catch (Exception ex)
            {
                playersServiceHost = null;
                Console.WriteLine("There is an issue with GameService" + ex.Message);
                Console.ReadKey();
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
