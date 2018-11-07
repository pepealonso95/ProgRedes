using System;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract]
    [Serializable]
    public class Player
    {
        [DataMember]
        public string Nickname { get; set; }

        [DataMember]
        public byte[] Image { get; set; }

        private bool loggedIn;
        public int Score { get; set; }

        public Player(string nickname)
        {
            this.Nickname = nickname;
            this.loggedIn = false;
            this.Score = 0;
        }

        public Player(string nickname, byte[] image)
        {
            this.Nickname = nickname;
            this.Image = image;
            this.loggedIn = false;
            this.Score = 0;
        }

        public void LogIn()
        {
            this.loggedIn = true;
        }

        public void LogOut()
        {
            this.loggedIn = false;
        }


        public bool IsLogged()
        {
            return this.loggedIn;
        }
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return this.Nickname.Equals(((Player)obj).Nickname);
            }
        }

        public override string ToString()
        {
            return this.Nickname;
        }

    }
}
