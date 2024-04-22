using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Cost { get; set; }
        public Action<Player>Effect { get; set; }

        
    }
}
