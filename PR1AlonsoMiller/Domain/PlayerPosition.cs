using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class PlayerPosition
    {
        public Player player;
        public int x;
        public int y;

        public PlayerPosition(Player player)
        {
            this.player = player;
            x = -1;
            y = -1;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return this.player.Equals(((PlayerPosition)obj).player);
            }
        }
    }
}
