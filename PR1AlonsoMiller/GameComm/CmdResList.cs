using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdResList
    {
        public const int FIXED_LENGTH = 10;

        public const string NO_VAR = "00000";

        public const string HEADER = "RES";

        public const string OK = "00";

        public const string REGISTER_INVALID = "01";

        public const string LOGIN_INVALID = "02";
        public const string SERVERFULL = "03";

        public const string UNKNOWN = "04";
        public const string NOTLOGGED = "05";
        public const string ALREADY_LOGGED = "06";

        public const string NOT_EXPECTING_IMG = "07";
        public const string EXPECTING_IMG = "08";



        public const string MATCHFINISHED = "09";
        public const string MATCHFULL = "10";
        public const string NOTINMATCH = "11";
        public const string INMATCH = "12";
        public const string PLAYERDEAD = "13";
        public const string OUT_OF_BOUNDS = "14";
        public const string OCCUPIED = "15";
        public const string DIDNT_SELECT = "16";


        public const string INVALID_WHILE_PLAYING = "20";
        public const string NOT_PLAYING = "21";


        public const string BROADCAST = "98";
        public const string EXIT = "99";
    }
}
