using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Domain
{
    public class EmptyPos : Character
    {

        public EmptyPos() : base() { }
        
        public override void TakeDamage(int damage)
        {
        }

        private bool ValidateDamage(int damage)
        {
            return false;
        }
    }
}
