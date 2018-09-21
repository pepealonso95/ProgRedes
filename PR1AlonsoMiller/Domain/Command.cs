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
        LOGOUT,
        JOINMATCH,
        SELECTCHARACTER,
        MOVE,
        ATTACK,
        EXIT,
        UNKNOWN

    }
    public abstract class Command
    {
        protected const int VAR_SIZE_LIMIT = 9999;
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

        public string EnterValidLengthString()
        {
            string entered = Console.ReadLine();
            int length = System.Text.ASCIIEncoding.Unicode.GetByteCount(entered);
            while (length > VAR_SIZE_LIMIT)
            {
                Console.WriteLine("Entered value exceeds limits, please re-enter");
                entered = Console.ReadLine();
                length = System.Text.ASCIIEncoding.Unicode.GetByteCount(entered);
            }
            return entered;
        }
    }
}
