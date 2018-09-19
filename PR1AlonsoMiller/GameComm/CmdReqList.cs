using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdReqList
    {
        public const int MAX_VAR_SIZE = 9999;
        public const string NAMEPICSEPARATOR = "</%#?>";

        public const string NO_VAR = "0000";

        public const string SERVERIP = "192.168.1.21";

        public const string HEADER = "REQ";

        public const string UNKNOWN = "00";
        public const string REGISTER = "01";
        public const string LOGIN = "02";
        public const string LOGOUT = "03";

        public const string JOINMATCH = "04";
        public const string SELECTCHARACTER = "05";

        public const string MOVECHARACTER = "06";

        public const string EXIT = "99";
    }
}
