using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Domain
{
    public abstract class Character
    {
        public Player player;
        protected int health;
        protected int attack;

        public Character()
        {
            player = null;
            health = 0;
            attack = 0;
        }
        
        public abstract void TakeDamage(int damage);

        public bool IsAlive()
        {
            return health > 0;
        }

        public int GetAttack()
        {
            return attack;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return this.player.Equals(((Character)obj).player);
            }
        }
    }
}
