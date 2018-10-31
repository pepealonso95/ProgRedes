using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace GameComm
{
    public interface IPlayerHandler
    {
        List<PlayerScore> GetPlayers();
    }
}
