using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieGame.Models
{
    public class Room
    {
        public string Name { get; set; }
        public Room North { get; set; }
        public Room South { get; set; }
        public Room East { get; set; }
        public Room West { get; set; }
        public List<Zombie>Zombies  { get; set; }
        public bool hasMysteryBox { get; set; }
        public bool ZombiesGenerated { get; set; }




    }
}
