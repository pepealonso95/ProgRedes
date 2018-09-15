using GameComm;
using System;
using Domain;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Commands
{
    public class CommandShowLogged:Command
    {
        public CommandShowLogged() : base(ServerCmd.SHOWLOGGED)
        {
        }

        public override string Run()
        {
            List<Player> players = PlayerList.GetLoggedPlayers();
            if (players.Count == 0)
            {
                Console.WriteLine("There are currently no logged players");
            }
            foreach (Player player in players)
            {
                Console.WriteLine(player.ToString());
            }
            return "Ok";
        }
        

    }
}
