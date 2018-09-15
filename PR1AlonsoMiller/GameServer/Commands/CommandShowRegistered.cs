using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Commands
{
    public class CommandShowRegistered:Command
    {
        public CommandShowRegistered() : base(ServerCmd.SHOWREGISTERED)
        {
        }
        public override string Run()
        {
            List<Player> players = PlayerList.GetInstance();
            if (players.Count == 0)
            {
                Console.WriteLine("There are currently no registered players");
            }
            foreach (Player player in players)
            {
                Console.WriteLine(player.ToString());
            }
            return "Ok";
        }
        

    }
}
