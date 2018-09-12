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
    }
}
