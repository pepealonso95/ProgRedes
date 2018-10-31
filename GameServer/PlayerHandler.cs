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
    }
}
