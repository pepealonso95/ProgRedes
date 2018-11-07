using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameComm;
using Domain;

namespace GameServer
{
    public class PlayerHandler : MarshalByRefObject, IPlayerHandler
    {
        public List<PlayerScore> GetPlayers()
        {
            return PlayerList.GetPlayers();
        }


        public string AddPlayer(string username)
        {
            Player player = new Player(username);
            if (PlayerList.PlayerRegister(player))
            {
                return "Added Succesfully";
            }
            else
            {
                return "Already Exists";
            }
        }
        public string ModifyPlayer(string oldUsername, string newUsername)
        {
            if (PlayerList.ModifyPlayer(oldUsername, newUsername))
            {
                return "Modified Succesfully";
            }
            else
            {
                return "Player doesnt exist";
            }
        }
        public string DeletePlayer(string username)
        {
            Player player = new Player(username);
            if (PlayerList.RemovePlayer(player))
            {
                return "Removed Succesfully";
            }
            else
            {
                return "Player doesnt exist";
            }
        }
    }
}
