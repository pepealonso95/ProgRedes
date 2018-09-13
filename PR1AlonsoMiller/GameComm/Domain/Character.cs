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
        
        public abstract void TakeDamage(int damage);

        public bool IsAlive()
        {
            return health > 0;
        }
    }
}
