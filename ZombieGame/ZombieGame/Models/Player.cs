using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public List<Item> Items { get; set; }
        public List<Weapon> Weapons { get; set; }
        public Room CurrentRoom { get; set; }
        public Weapon CurrentWeapon { get; set; }
        public int Points { get; set; }
        public bool isDown { get; set; }
        public bool GameOver { get; set; }
    }

}
