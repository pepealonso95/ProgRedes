using System;
using System.Runtime.Serialization;

namespace Domain
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public string Nickname { get; }

        [DataMember]
        public byte[] Image { get; }

        public Player(string nickname)
        {
            this.Nickname = nickname;
        }

        public Player(string nickname, byte[] image)
        {
            this.Nickname = nickname;
            this.Image = image;
        }

        public override bool Equals(object obj)
        {
            return this.Nickname.Equals(((Player)obj).Nickname);
        }
    }
}
