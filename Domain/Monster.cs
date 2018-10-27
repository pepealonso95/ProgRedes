using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Domain
{
    public class Monster : Character
    {

        public Monster(Player monsterPlayer)
        {
            player = monsterPlayer;
            health = RoleValues.MONSTER_HEALTH;
            attack = RoleValues.MONSTER_ATTACK;
        }
        public override void TakeDamage(int damage)
        {
            if (this.IsAlive())
            {
                health -= damage;
            }
        }
    }
}
