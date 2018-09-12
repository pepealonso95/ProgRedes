using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdReqList
    {
        public const string NAMEPICSEPARATOR = "</%#?>";

        public const string NO_VAR = "0000";

        public const string SERVERIP = "192.168.1.46";

        public const string HEADER = "REQ";

        public const string UNKNOWN = "00";
        public const string REGISTER = "01";
        public const string LOGIN = "02";

        public const string EXIT = "99";
    }
}
