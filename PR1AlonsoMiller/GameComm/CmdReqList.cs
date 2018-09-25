using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdReqList
    {
        public const int MAX_VAR_SIZE = 99999;
        public const string NAMEPICSEPARATOR = "</%#?>";
        public const string IMAGEREQSEPARATOR = "<&/$·>";

        public const string NO_VAR = "00000";

        public const string HEADER = "REQ";

        public const string UNKNOWN = "00";
        public const string REGISTER = "01";
        public const string LOGIN = "02";
        public const string LOGOUT = "03";

        public const string JOINMATCH = "04";
        public const string SELECTCHARACTER = "05";

        public const string MOVECHARACTER = "06";
        public const string ATTACKCHARACTER = "07";


        public const string PICTURE = "20";

        public const string EXIT = "99";
    }
}
