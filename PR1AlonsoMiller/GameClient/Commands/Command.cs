using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Commands
{
    public enum RequestCmd
    {
        REGISTER,
        LOGIN,
        EXIT,
        UNKNOWN

    }
    public abstract class Command
    {
        public readonly RequestCmd Request;

        public Command(RequestCmd request)
        {
            Request = request;
        }

        public abstract string Run();
    }
}
