using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class Match
    {
        private bool finished = false;
        private Character[,] Terrain = new Character[8,8];
        private List<Character> DeadPlayers = new List<Character>();
        private static readonly object turnLock = new object();
        public void AddSurvivor(Player player)
        {
            Character newSurvivor = new Survivor(player);
            CreateCharacterThread(newSurvivor);
        }
        public void AddMonster(Player player)
        {
            Character newMonster = new Monster(player);
            CreateCharacterThread(newMonster);
        }

        private void CreateCharacterThread(Character character)
        {
            Thread myThread = new Thread(() => PlayerCommand(character));
            myThread.Start();
        }

        public void PlayerCommand(Character player)
        {
            while (!finished && player.IsAlive())
            {
                Monitor.Enter(turnLock);
                Monitor.Exit(turnLock);
            }
            if (!player.IsAlive())
            {
                DeadPlayers.Add(player);
            }
        }
    }
}
