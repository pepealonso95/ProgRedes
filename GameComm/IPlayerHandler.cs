using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace GameComm
{
    public interface IPlayerHandler
    {
        List<PlayerScore> GetPlayers();
        string AddPlayer(string username);
        string ModifyPlayer(string oldUsername, string newUsername);
        string DeletePlayer(string username);
    }
}
