using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon:Item
    {
        
        public int MaxDamage { get; set; }
        public int MinDamage { get; set; }
        public Weapon(int id,string name,string namePlural,int minDamage,int maxDamage) : base(id, name, namePlural)
        {
            minDamage = MinDamage;
            maxDamage = MaxDamage;
        }
    }
}
