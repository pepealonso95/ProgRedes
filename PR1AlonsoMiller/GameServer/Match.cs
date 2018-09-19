using Domain;
using GameComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class Match
    {
        public const int MAX_ACTIVE_PLAYERS = 32;
        private string result = "No result";
        private static Match instance;
        public bool Finished { get; private set; }
        private int playerCount = 0;
        private List<PlayerPosition> Playing = new List<PlayerPosition>();
        private Character[,] Terrain = new Character[8,8];
        private List<Player> DeadPlayers = new List<Player>();
        private static readonly object turnLock = new object();

        private Match() { }

        public static Match Instance()
        {
            if (instance == null)
            {
                instance = new Match();
                instance.Finished = true;
            }
            return instance;
        }

        private void ResetMatch()
        {
            for (int i = 0; i < Terrain.GetLength(0); i++)
            {
                for (int j = 0; j < Terrain.GetLength(1); j++)
                {
                    Terrain[i, j] = new Survivor();
                }
            }
            result = "No result";
            playerCount = 0;
            DeadPlayers = new List<Player>();
            Playing  = new List<PlayerPosition>();
        }

        public static void StartMatch()
        {
            instance.ResetMatch();
            ServerMain.BroadcastMessage("MATCH STARTS IN 10 SECONDS");
            Thread.Sleep(10000);
            ServerMain.BroadcastMessage("MATCH STARTED");
            instance.Finished = false;
            Thread.Sleep(180000);
            instance.Finished = true;
            instance.Results();
        }

        private void Results()
        {
            bool survivorsAlive = false;
            List<Player> monsters = new List<Player>();
            FindAllInTerrain(survivorsAlive, monsters);
            if (survivorsAlive)
            {
                result = "SURVIVORS WIN!";
            }
            else if (monsters.Count == 1)
            {
                result = monsters[0].Nickname + " WINS!";
            }
            else
            {
                result = "NOBODY WINS";
            }
            ServerMain.BroadcastMessage(result);
        }

        private void FindAllInTerrain(bool survivorsAlive, List<Player> monsters)
        {
            for (int i = 0; i < Terrain.GetLength(0); i++)
            {
                for (int j = 0; j < Terrain.GetLength(1); j++)
                {
                    if (Terrain[i, j].GetAttack() == RoleValues.SURVIVOR_ATTACK)
                    {
                        survivorsAlive = true;
                        return;
                    }
                    else if (Terrain[i, j].GetAttack() == RoleValues.MONSTER_ATTACK)
                    {
                        monsters.Add(Terrain[i, j].player);
                    }
                }
            }
        }

        public string AddPlayer(Player player)
        {
            Monitor.Enter(turnLock);
            string result = "";
            PlayerPosition playerToJoin = new PlayerPosition(player);
            if (playerCount >= MAX_ACTIVE_PLAYERS)
            {
                result = CmdResList.MATCHFULL;
            }
            else if (!Playing.Contains(playerToJoin))
            {
                result = CmdResList.INMATCH;
            }
            else
            {
                Playing.Add(playerToJoin);
                playerCount++;
                result = "Succesfully added";
            }
            Monitor.Exit(turnLock);
            return result;
        }
        
        public string AddCharacter(Character character)
        {
            Monitor.Enter(turnLock);
            string result = "";
            PlayerPosition playingChar = Playing.FirstOrDefault(p => p.player.Equals(character.player));
            if (playingChar==null)
            {
                result = CmdResList.NOTINMATCH;
            }
            else if (DeadPlayers.Contains(character.player))
            {
                result = CmdResList.PLAYERDEAD;
            }
            else if(playingChar.x!=-1&& playingChar.y != -1)
            {
                result = CmdResList.INMATCH;
            }
            else 
            {
                result = AddToTerrain(character);
            }
            Monitor.Exit(turnLock);
            return result;
        }

        private string AddToTerrain(Character character)
        {
            string position = "";
            bool added = false;
            bool[,] usedPositions = new bool[8, 8];
            while (!added)
            {
                Random random = new Random();
                int row = random.Next(0, 7);
                int column = random.Next(0, 7);
                if (!usedPositions[row, column])
                {
                    usedPositions[row, column] = true;
                    List<Character> adjacentCharacters = FindAdjacentCharacters(row, column);
                    if (adjacentCharacters.Count == 0)
                    {
                        position = UpdatePosition(character, row, column);
                        added = true;
                    }
                }
            }
            return position;

        }

        private string UpdatePosition(Character character, int row, int column)
        {
            Terrain[row, column] = character;
            PlayerPosition charPosition = Playing.FirstOrDefault(p=>p.player.Equals(character.player));
            charPosition.x = row;
            charPosition.y = column;
            return "Character in [" + row + "," + column + "]";
        }

        private List<Character> FindAdjacentCharacters(int row, int column)
        {
            List<Character> adjacentCharacters = new List<Character>();
            CheckCharacterBelow(row, column, adjacentCharacters);
            CheckCharacterAbove(row, column, adjacentCharacters);
            CheckCharacterRight(row, column, adjacentCharacters);
            CheckCharacterLeft(row, column, adjacentCharacters);
            return adjacentCharacters;
        }

        private void CheckCharacterLeft(int row, int column, List<Character> adjacentCharacters)
        {
            if (column - 1 > 0 && PositionIsOccupied(Terrain[row, column - 1]))
            {
                adjacentCharacters.Add(Terrain[row, column - 1]);
            }
        }

        private void CheckCharacterRight(int row, int column, List<Character> adjacentCharacters)
        {
            if (column + 1 < Terrain.Length && PositionIsOccupied(Terrain[row, column + 1]))
            {
                adjacentCharacters.Add(Terrain[row, column + 1]);
            }
        }

        private void CheckCharacterAbove(int row, int column, List<Character> adjacentCharacters)
        {
            if (row - 1 > 0 && PositionIsOccupied(Terrain[row - 1, column]))
            {
                adjacentCharacters.Add(Terrain[row - 1, column]);
            }
        }

        private void CheckCharacterBelow(int row, int column, List<Character> adjacentCharacters)
        {
            if (row + 1 < Terrain.Length && PositionIsOccupied(Terrain[row + 1, column]))
            {
                adjacentCharacters.Add(Terrain[row + 1, column]);
            }
        }

        public string PlayerCommand(Player player, string command)
        {
            if (Finished)
            {
                return "Must wait for match to start.";
            }
            else if (DeadPlayers.Contains(player))
            {
                return "You are Dead, must wait for the next match.";
            }
            else
            {
                string response = "";
                Monitor.Enter(turnLock);
                Monitor.Exit(turnLock);
                return response;
            }
        }

        private bool PositionIsOccupied(Character position)
        {
            return position.GetAttack() != 0;
        }
    }
}
