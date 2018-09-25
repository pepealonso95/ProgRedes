using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdResList
    {
        public const int FIXED_LENGTH = 8;

        public const string NO_VAR = "00000";


        public const string OK = "100";

        public const string REGISTER_INVALID = "101";

        public const string LOGIN_INVALID = "102";
        public const string SERVERFULL = "103";

        public const string UNKNOWN = "104";
        public const string NOTLOGGED = "105";
        public const string ALREADY_LOGGED = "106";

        public const string NOT_EXPECTING_IMG = "107";
        public const string EXPECTING_IMG = "108";



        public const string MATCHFINISHED = "109";
        public const string MATCHFULL = "110";
        public const string NOTINMATCH = "111";
        public const string INMATCH = "112";
        public const string PLAYERDEAD = "113";
        public const string OUT_OF_BOUNDS = "114";
        public const string OCCUPIED = "115";
        public const string DIDNT_SELECT = "116";


        public const string INVALID_WHILE_PLAYING = "120";
        public const string NOT_PLAYING = "121";


        public const string BROADCAST = "198";
        public const string EXIT = "199";
    }
}
