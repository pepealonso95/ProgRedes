﻿using Domain;
using GameComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GameServer.Log;

namespace GameServer
{
    public class Match
    {
        public const int MAX_ACTIVE_PLAYERS = 32;
        public const int WIN_POINTS = 15;
        public const int KILL_POINTS = 5;
        private string result = "No result";
        private static Match instance;
        public static string duration = "180000";
        public bool Finished { get; private set; }
        private int playerCount = 0;
        private List<PlayerPosition> Playing = new List<PlayerPosition>();
        private Character[,] Terrain = new Character[8,8];
        private List<Character> DeadPlayers = new List<Character>();
        public Logger logger = new Logger();
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
                    Terrain[i, j] = new EmptyPos();
                }
            }
            result = "No result";
            playerCount = 0;
            DeadPlayers = new List<Character>();
            Playing  = new List<PlayerPosition>();
            logger.CleanLog();
            LogEntry log = new LogEntry()
            {
                User = "START",
                Action = "Match Started"
            };
            logger.LogNewEntry(log);
            PlayerList.NextMatch();
        }

        public static void StartMatch()
        {
            instance.ResetMatch();
            ServerMain.BroadcastMessage("MATCH STARTS IN 10 SECONDS");
            Thread.Sleep(10000);
            ServerMain.BroadcastMessage("MATCH STARTED");
            instance.Finished = false;
            Thread.Sleep(Int32.Parse(duration));
            instance.Finished = true;
            instance.Results();
        }

        private void Results()
        {
            bool survivorsAlive = false;
            bool monsterWon = false;
            List<Monster> monsters = new List<Monster>();
            FindAllInTerrain(out survivorsAlive, monsters);
            if (survivorsAlive)
            {
                result = "SURVIVORS WIN!";
            }
            else if (monsters.Count == 1)
            {
                result = monsters[0].player.Nickname + " WINS!";
                monsters[0].player.Score+=WIN_POINTS;
                monsterWon = true;
            }
            else
            {
                result = "NOBODY WINS";
            }
            AddScores(survivorsAlive, monsterWon);
            LogEntry entry = new LogEntry()
            {
                User = "Match Finished",
                Action = result
            };
            logger.LogNewEntry(entry);
            logger.UpdateQueue();
            ServerMain.BroadcastMessage(result);
        }


        private void AddScores(bool survivorsWin, bool monsterWon)
        {
            for (int i = 0; i < Terrain.GetLength(0); i++)
            {
                for (int j = 0; j < Terrain.GetLength(1); j++)
                {
                    if (Terrain[i, j].GetAttack() == RoleValues.SURVIVOR_ATTACK)
                    {
                        PlayerList.AddScore(Terrain[i, j], survivorsWin);
                    }
                    else if (Terrain[i, j].GetAttack() == RoleValues.MONSTER_ATTACK)
                    {
                        PlayerList.AddScore(Terrain[i, j], monsterWon);
                    }
                }
            }
            foreach (Character deadChar in DeadPlayers)
            {
                PlayerList.AddScore(deadChar, false);
            }
        }

        private void FindAllInTerrain(out bool survivorsAlive , List<Monster> monsters)
        {
            survivorsAlive = false;
            for (int i = 0; i < Terrain.GetLength(0); i++)
            {
                for (int j = 0; j < Terrain.GetLength(1); j++)
                {
                    if (Terrain[i, j].GetAttack() == RoleValues.SURVIVOR_ATTACK)
                    {
                        survivorsAlive = true;
                        Terrain[i, j].player.Score += WIN_POINTS;
                    }
                    else if (Terrain[i, j].GetAttack() == RoleValues.MONSTER_ATTACK)
                    {
                        monsters.Add((Monster)Terrain[i, j]);
                    }
                }
            }
        }

        public string AddPlayer(Player player)
        {
            string result = "";
            if (Finished)
            {
                return CmdResList.MATCHFINISHED;
            }
            Monitor.Enter(turnLock);
            if (!Finished)
            {
                PlayerPosition playerToJoin = new PlayerPosition(player);
                if (playerCount >= MAX_ACTIVE_PLAYERS)
                {
                    result = CmdResList.MATCHFULL;
                }
                else if (Playing.Contains(playerToJoin))
                {
                    result = CmdResList.INMATCH;
                }
                else
                {
                    Playing.Add(playerToJoin);
                    playerCount++;
                    playerToJoin.player.Score = 0;
                    result = "Succesfully added";
                    LogEntry log = new LogEntry()
                    {
                        User = playerToJoin.player.Nickname,
                        Action = "Joined"
                    };
                    logger.LogNewEntry(log);
                }
            }
            Monitor.Exit(turnLock);
            return result;
        }
        
        public string AddCharacter(Character character)
        {
            if (character == null)
            {
                return CmdResList.UNKNOWN;
            }
            if (Finished)
            {
                return CmdResList.MATCHFINISHED;
            }
            if (DeadPlayers.Contains(character))
            {
                return CmdResList.PLAYERDEAD;
            }
            string result = "";
            Monitor.Enter(turnLock);
            if (!Finished)
            {
                PlayerPosition playingChar = Playing.FirstOrDefault(p => p.player.Equals(character.player));
                if (playingChar == null)
                {
                    result = CmdResList.NOTINMATCH;
                }
                else if (DeadPlayers.Contains(character))
                {
                    result = CmdResList.PLAYERDEAD;
                }
                else if (playingChar.x != -1 && playingChar.y != -1)
                {
                    result = CmdResList.INMATCH;
                }
                else
                {
                    result = AddToTerrain(character);
                    LogEntry log = new LogEntry()
                    {
                        User = character.player.Nickname,
                        Action = "Entered " + result
                    };
                    logger.LogNewEntry(log);
                }
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
            return "Character in vertical:" + row + " horizontal:" + column;
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
            if (column + 1 < Terrain.GetLength(1) && PositionIsOccupied(Terrain[row, column + 1]))
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
            if (row + 1 < Terrain.GetLength(0) && PositionIsOccupied(Terrain[row + 1, column]))
            {
                adjacentCharacters.Add(Terrain[row + 1, column]);
            }
        }

        public string Move(Player player, string directions)
        {
            if (Finished)
            {
                return CmdResList.MATCHFINISHED;
            }
            else
            {
                string response = "";
                Monitor.Enter(turnLock);
                if (!Finished)
                {
                    PlayerPosition playingChar = Playing.FirstOrDefault(p => p.player.Equals(player));
                    if (playingChar == null)
                    {
                        response = CmdResList.NOTINMATCH;
                    }
                    else if (playingChar.x==-1|| playingChar.y==-1)
                    {
                        response = CmdResList.DIDNT_SELECT;
                    }
                    else if (DeadPlayers.Where(c=>c.player.Equals(player)).Count()>0)
                    {
                        response = CmdResList.PLAYERDEAD;
                    }
                    else
                    {
                        response = MoveCharacter(playingChar, directions[0]);
                        if (MoveOk(response) )
                        {
                            if (directions.Length == 2)
                            {
                                string nextResponse = MoveCharacter(playingChar, directions[1]);
                                if (MoveOk(nextResponse))
                                {
                                    response = nextResponse;
                                }
                                else
                                {
                                    response += ", second move invalid";
                                }
                            }
                            response += InformNearCharacters(playingChar);
                            LogEntry log = new LogEntry()
                            {
                                User = playingChar.player.Nickname,
                                Action = "Moved to vertical:" + playingChar.x + " horizontal:" + playingChar.y
                            };
                            logger.LogNewEntry(log);
                        }
                    }
                }
                Monitor.Exit(turnLock);
                return response;
            }
        }

        private string InformNearCharacters(PlayerPosition playingChar)
        {
            List<Character> nearCharacters = FindAdjacentCharacters(playingChar.x, playingChar.y);
            string response = ". Nearby characters: ";
            foreach(Character character in nearCharacters)
            {
                if (character.GetAttack() == RoleValues.SURVIVOR_ATTACK)
                {
                    response+=" Survivor,";
                }
                else if (character.GetAttack() == RoleValues.MONSTER_ATTACK)
                {
                    response += " Monster,";
                }
            }
            if (nearCharacters.Count == 0)
            {
                response += " None";
            }
            return response;
        }

        private bool MoveOk(string response)
        {
            return response != CmdResList.UNKNOWN && response != CmdResList.OCCUPIED && response != CmdResList.OUT_OF_BOUNDS;
        }

        private string MoveCharacter(PlayerPosition playingChar, char direction)
        {
            int nextRow = playingChar.x;
            int nextColumn = playingChar.y;
            if (direction == 'U')
            {
                nextRow = nextRow - 1;
            }
            else if (direction == 'D')
            {
                nextRow = nextRow + 1;
            }
            else if (direction == 'R')
            {
                nextColumn = nextColumn + 1;
            }
            else if (direction == 'L')
            {
                nextColumn = nextColumn - 1;
            }
            else
            {
                return CmdResList.UNKNOWN;
            }
            return ValidateMovement(playingChar, nextRow, nextColumn);
        }

        private string ValidateMovement(PlayerPosition playingChar, int nextRow, int nextColumn)
        {
            if (nextRow < 0 || nextRow >= Terrain.GetLength(0) || nextColumn < 0 || nextColumn >= Terrain.GetLength(1))
            {
                return CmdResList.OUT_OF_BOUNDS;
            }
            else if (PositionIsOccupied(Terrain[nextRow, nextColumn]))
            {
                return CmdResList.OCCUPIED;
            }
            else
            {
                Terrain[nextRow, nextColumn] = Terrain[playingChar.x, playingChar.y];
                Terrain[playingChar.x, playingChar.y] = new EmptyPos();
                playingChar.x = nextRow;
                playingChar.y = nextColumn;
                return "Character in vertical:" + nextRow + " horizontal:" + nextColumn;
            }
        }

        private bool PositionIsOccupied(Character position)
        {
            return position.GetAttack() > 0;
        }

        public string Attack(Player player)
        {
            if (Finished)
            {
                return CmdResList.MATCHFINISHED;
            }
            else
            {
                string response = "";
                Monitor.Enter(turnLock);
                if (!Finished)
                {
                    PlayerPosition playingChar = Playing.FirstOrDefault(p => p.player.Equals(player));
                    if (playingChar == null)
                    {
                        response = CmdResList.NOTINMATCH;
                    }
                    else if (playingChar.x == -1 || playingChar.y == -1)
                    {
                        response = CmdResList.DIDNT_SELECT;
                    }
                    else if (DeadPlayers.Where(c=>c.player.Equals(player)).Count()>0)
                    {
                        response = CmdResList.PLAYERDEAD;
                    }
                    else
                    {
                            response = AttackNearCharacters(playingChar);
                    }
                }
                Monitor.Exit(turnLock);
                return response;
            }
        }

        private string AttackNearCharacters(PlayerPosition playingChar)
        {
            List<Character> nearCharacters = FindAdjacentCharacters(playingChar.x, playingChar.y);
            Character myCharacter = Terrain[playingChar.x, playingChar.y];
            string response = ". Attacked: ";
            foreach (Character character in nearCharacters)
            {
                if (character.GetAttack() == RoleValues.SURVIVOR_ATTACK)
                {
                    if(myCharacter.GetAttack()!= RoleValues.SURVIVOR_ATTACK)
                    {
                        character.TakeDamage(myCharacter.GetAttack());
                        response += " Survivor";

                        LogEntry log = new LogEntry()
                        {
                            User = myCharacter.player.Nickname,
                            Action = "Attacked"
                        };
                        logger.LogNewEntry(log);


                        log = new LogEntry()
                        {
                            User = character.player.Nickname,
                            Action = "Was Attacked"
                        };
                        logger.LogNewEntry(log);
                    }
                }
                else if (character.GetAttack() == RoleValues.MONSTER_ATTACK)
                {
                    character.TakeDamage(myCharacter.GetAttack());
                    response += " Monster";

                    LogEntry log = new LogEntry()
                    {
                        User = myCharacter.player.Nickname,
                        Action = "Attacked"
                    };
                    logger.LogNewEntry(log);


                    log = new LogEntry()
                    {
                        User = character.player.Nickname,
                        Action = "Was Attacked"
                    };
                    logger.LogNewEntry(log);
                }
                if (!character.IsAlive())
                {
                    response += Kill(character.player);
                    myCharacter.player.Score+=KILL_POINTS;
                }
                response += ",";
            }
            if (nearCharacters.Count == 0)
            {
                response += " None";
            }
            return response;
        }
        private string Kill(Player player)
        {
            string response = "";
            if (!Finished)
            {
                PlayerPosition playerToKill = Playing.FirstOrDefault(p => p.player.Equals(player));
                if (playerToKill == null)
                {
                    response = CmdResList.NOTINMATCH;
                }
                else if (playerToKill.x == -1 || playerToKill.y == -1)
                {
                    response = CmdResList.DIDNT_SELECT;
                }
                else if (DeadPlayers.Where(c=>c.player.Equals(player)).Count()>0)
                {
                    response = CmdResList.PLAYERDEAD;
                }
                else
                {
                    DeadPlayers.Add(Terrain[playerToKill.x, playerToKill.y]);
                    Terrain[playerToKill.x, playerToKill.y] = new EmptyPos();
                    response = "Killed (" + player.Nickname + ")";
                    LogEntry log = new LogEntry()
                    {
                        User = player.Nickname,
                        Action = "Was Killed"
                    };
                    logger.LogNewEntry(log);
                }
                return response;
            }
            else
            {
                return CmdResList.MATCHFINISHED;
            }
        }

        public void PlayerKill(Player player)
        {
            if (player == null)
            {
                return;
            }
            if (Finished)
            {
                return;
            }
            if (DeadPlayers.Where(c=>c.player.Equals(player)).Count()>0)
            {
               return;
            }
            PlayerPosition playerToKill = Playing.FirstOrDefault(p => p.player.Equals(player));
            if (playerToKill == null)
            {
                return;
            }
            else if (playerToKill.x == -1 || playerToKill.y == -1)
            {
                return;
            }
            Monitor.Enter(turnLock);
            Kill(player);
            Monitor.Exit(turnLock);
        }

    }
}
