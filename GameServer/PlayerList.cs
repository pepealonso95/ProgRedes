using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace GameServer
{
    public class PlayerList
    {
        private static readonly object useLock = new object();

        private static List<Player> players;
        private static List<PlayerScore> scores;
        private static int match;
        private PlayerList()
        {
            players = new List<Player>();
            scores = new List<PlayerScore>();
        }

        public static List<Player> GetInstance()
        {
            Monitor.Enter(useLock);
            if (players == null)
            {
                players = new List<Player>();
            }
            Monitor.Exit(useLock);
            return players;
        }


        public static List<PlayerScore> GetScoresInstance()
        {
            Monitor.Enter(useLock);
            if (scores == null)
            {
                scores = new List<PlayerScore>();
                match = 0;
            }
            Monitor.Exit(useLock);
            return scores;
        }

        public static List<Player> GetLoggedPlayers()
        {
            Monitor.Enter(useLock);
            List<Player> loggedPlayers = new List<Player>();
            foreach (Player player in GetInstance())
            {
                if (player.IsLogged())
                {
                    loggedPlayers.Add(player);
                }
            }
            Monitor.Exit(useLock);
            return loggedPlayers;
        }


        public static List<PlayerScore> GetPlayers()
        {
            Monitor.Enter(useLock);
            List<PlayerScore> playerScores = new List<PlayerScore>();
            foreach (PlayerScore player in GetScoresInstance())
            {
                playerScores.Add(player);
            }
            Monitor.Exit(useLock);
            return playerScores;
        }


        public static Player PlayerLogin(string nickname)
        {
            Monitor.Enter(useLock);
            Player logged = GetInstance().Find(p => p.Nickname == nickname&&!p.IsLogged());
            if (logged!=null)
            {
                logged.LogIn();
            }
            Monitor.Exit(useLock);
            return logged;
        }


        public static bool PlayerRegister(Player player)
        {
            bool result = false;
            Monitor.Enter(useLock);
            if (!GetInstance().Contains(player))
            {
                GetInstance().Add(player);
                result = true;
            }
            Monitor.Exit(useLock);
            return result;
        }

        public static Player PlayerLogout(Player player)
        {
            Player logged = null;
            Monitor.Enter(useLock);
            if (GetInstance().Contains(player))
            {
                logged = GetInstance().Find(p => p.Nickname == player.Nickname);
                logged.LogOut();
            }
            Monitor.Exit(useLock);
            return logged;
        }

        public static void SetImage(Player registering, string registeringImg)
        {
            Monitor.Enter(useLock);
            if (GetInstance().Contains(registering))
            {
                Player imgChange = GetInstance().Find(p => p.Nickname == registering.Nickname);
                imgChange.Image = Encoding.ASCII.GetBytes(registeringImg);
            }
            Monitor.Exit(useLock);
        }

        public static void AddScore(Character character, bool win)
        {
            string role = GetRole(character.GetAttack());
            PlayerScore score = new PlayerScore()
            {
                User = character.player.Nickname,
                Survived = win,
                Score = character.player.Score,
                Role = role,
                Date = DateTime.Now,
                Match = match
            };
            GetScoresInstance().Add(score);
        }

        private static string GetRole(int attack)
        {
            if (attack == RoleValues.MONSTER_ATTACK)
            {
                return "MONSTER";
            }
            else{
                return "SURVIVOR";
            }
        }

        public static void NextMatch()
        {
            Monitor.Enter(useLock);
            if (match==null)
            {
                match = 0;
            }
            match++;
            Monitor.Exit(useLock);
        }


        public static bool ModifyPlayer(string oldUsername, string newUsername)
        {
            bool result = false;
            Monitor.Enter(useLock);
            Player found = GetInstance().Find(p => p.Nickname == oldUsername);
            if (found != null)
            {
                result = true;
                found.Nickname = newUsername;
            }
            Monitor.Exit(useLock);
            return result;
        }


        public static bool RemovePlayer(Player player)
        {
            bool result = false;
            Monitor.Enter(useLock);
            result = GetInstance().Remove(player);
            Monitor.Exit(useLock);
            return result;
        }
    }
}
