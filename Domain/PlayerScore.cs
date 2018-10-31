using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    [Serializable]
    public class PlayerScore
    {
        public int Score { get; set; }
        public bool Survived { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public DateTime Date { get; set; }
        public int Match { get; set; }
    }
}
