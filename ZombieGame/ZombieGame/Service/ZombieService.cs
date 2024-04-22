using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieGame.Models;

namespace ZombieGame.Service
{
    public class ZombieService
    {
        private PlayerService playerService { get; set; }
        public ZombieService(PlayerService ps) {
            playerService = ps;
        }
        public void GenerateZombies(Room room)
        {
           
            if (room.Zombies == null)
            {
                room.Zombies = new List<Zombie>();
            }

            var random = new Random();
            int zombieCount = random.Next(5, 15);

            for (int i = 0; i < zombieCount; i++)
            {
                Zombie zombie = new Zombie
                {
                    Health = 100
                };
                room.Zombies.Add(zombie); 
            }
        }

        public void ZombieAttack(Player player, Room room)
        {
            foreach (var zombie in room.Zombies)
            {
                if (zombie.Health > 0)
                {
                    int attack = new Random().Next(1, 5); 
                    player.Health -= attack;

                    Console.WriteLine($"A zombie attacked and hit you, damage done by the zombie was {attack}");

                    if (player.Health <= 0) 
                    {
                        Console.WriteLine($"Your health is {player.Health}, you're knocked down");
                        playerService.Revive(); 
                    }
                }
            }
        }
    }
}
