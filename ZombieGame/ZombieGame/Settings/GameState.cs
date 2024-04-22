using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieGame.Models;

namespace ZombieGame.Settings
{
    [Serializable]
    public class GameState
    {
        public Player Player { get; set; }
        public Room CurrentRoom { get; set; }
        public bool IsGameOver { get; set; }
        public Weapon CurrentWeapon { get; set; }
        public List<Room>Rooms{ get; set; }
        public List<Item> Items { get; set; }
        public List<Zombie>Zombies { get; set; }
        public int TotalZombies { get; set; }
        

    }
}
