using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class PlayerList
    {
        private static List<Player> players;

        private PlayerList()
        {
            players = new List<Player>();
        }

        public static List<Player> GetInstance()
        {
            if (players == null)
                players = new List<Player>();
            return players;
        }

        public static List<Player> GetLoggedPlayers()
        {
            List<Player> loggedPlayers = new List<Player>();
            foreach (Player player in players)
            {
                if (player.IsLogged())
                {
                    loggedPlayers.Add(player);
                }
            }
            return loggedPlayers;
        }
    }
}
