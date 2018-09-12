using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Domain
{
    public class Survivor : Character
    {

        public Survivor(Player survivorPlayer)
        {
            player = survivorPlayer;
            health = RoleValues.SURVIVOR_HEALTH;
            attack = RoleValues.SURVIVOR_ATTACK;
        }
        public override void TakeDamage(int damage)
        {
            if (ValidateDamage(damage) && IsAlive())
            {
                health -= damage;
            }
        }

        private bool ValidateDamage(int damage)
        {
            return damage != RoleValues.SURVIVOR_ATTACK;
        }
    }
}
