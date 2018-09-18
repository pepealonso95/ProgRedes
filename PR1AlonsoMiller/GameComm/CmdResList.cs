﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameComm
{
    public static class CmdResList
    {
        public const int FIXED_LENGTH = 9;

        public const string NO_VAR = "0000";

        public const string HEADER = "RES";

        public const string OK = "00";

        public const string REGISTER_INVALID = "01";

        public const string LOGIN_INVALID = "02";
        public const string SERVERFULL = "03";


        public const string BROADCAST = "98";
        public const string EXIT = "99";
    }
}