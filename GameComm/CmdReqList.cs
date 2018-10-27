using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdReqList
    {
        public const int MAX_VAR_SIZE = 99999;

        public const int REQLIMIT = 100;

        public const char NAMEPICSEPARATOR = '§';
        public const string IMAGEREQSEPARATOR = "<&/$·>";

        public const string NO_VAR = "00000";
        

        public const string UNKNOWN = "000";
        public const string REGISTER = "001";
        public const string LOGIN = "002";
        public const string LOGOUT = "003";

        public const string JOINMATCH = "004";
        public const string SELECTCHARACTER = "005";

        public const string MOVECHARACTER = "006";
        public const string ATTACKCHARACTER = "007";


        public const string PICTURE = "020";

        public const string EXIT = "099";
    }
}
