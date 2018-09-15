using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{

    public enum ServerCmd
    {
        SHOWREGISTERED,
        SHOWLOGGED,
        STARTMATCH,
        EXIT,
        UNKNOWN

    }
    public enum RequestCmd
    {
        REGISTER,
        LOGIN,
        EXIT,
        UNKNOWN

    }
    public abstract class Command
    {
        protected readonly RequestCmd Request;
        protected readonly ServerCmd Server;

        public Command(RequestCmd request)
        {
            Request = request;
        }
        public Command(ServerCmd server)
        {
            Server = server;
        }

        public abstract string Run();
    }
}
