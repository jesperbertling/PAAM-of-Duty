using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZombieGame.Helpers;
using ZombieGame.Models;
using ZombieGame.Settings;

namespace ZombieGame.Service
{
    public class PlayerService
    {
        public Player player { get; set; }
        public List<Item>Items { get; set; }
        private ItemService itemService { get; set; }
        private WeaponService weaponService { get; set; }
        private RoomService roomService { get; set; }


        public PlayerService(WeaponService ws, RoomService rs)
        {
            player = new Player();
         
            weaponService = ws;
            weaponService.InitializeWeapon(player);
            rs.CreateRooms();
        }

        public void DownPlayer()
        {
            if (player.Health == 0 && !HasItem("Self revive")) {
                player.isDown = true;
                player.GameOver = true;

                Console.WriteLine("Your health is 0 and you died..");
                GameHelper.DeleteSaveFile();
            }
            else
            {
                Revive();
            }
            
        }
        public void Revive()
        {
            if(HasItem("Self revive") && !player.isDown){
                var item = player.Items.FirstOrDefault(i => i.Name == "Self revive");

                player.isDown = false;
                player.GameOver = false;
                RemoveFromInventory(item);

            }
        }
        public bool HasItem(string itemName)
        {
            if (Items == null)
            {
                return false;
            }
            return Items.Any(x => x.Name.ToLower() == itemName.ToLower());
        } 

        public void RemoveFromInventory(Item item)
        {
            bool wasRemoved = player.Items.Remove(item);
            if(wasRemoved)
            {
                Console.WriteLine($"The item {item.Name} was removed from {player.Name}'s inventory");

            }
            else
            {
                Console.WriteLine("No item was found in the ");
            }
        }
        public Player CreateNewPlayer(string name)
        {
            var newPlayer = new Player
            {
                Name = name,
                Health = 100, // Startvärde
                Items = new List<Item>(),
                Weapons = new List<Weapon>(),
                Points = 0,
                CurrentWeapon = weaponService.GetFirstWeapon(), // Startvapen
                CurrentRoom = null, // Uppdateras senare
                isDown = false,
                GameOver = false
            };

            return newPlayer;
        
    }
        public void InteractWithMysteryBox(Room currentRoom)
        {
            if (currentRoom.Name == "Lounge") // Säkerställ att det är rätt rum
            {
                Console.WriteLine("Welcome to the Mystery Box! Would you like to buy a random weapon for 100 points? (yes/no)");
                var answer = Console.ReadLine().Trim().ToLower();

                if (answer == "yes")
                {
                    if (player.Points >= 100)
                    {
                        var availableWeapons = weaponService.GetAllWeapons();
                        var currentWeaponNames = player.Weapons.Select(w => w.Name).ToList();

                        // Ta bort vapen som spelaren redan har
                        var possibleWeapons = availableWeapons
                            .Where(w => !currentWeaponNames.Contains(w.Name))
                            .ToList();

                        if (possibleWeapons.Any())
                        {
                            var random = new Random();
                            var chosenWeapon = possibleWeapons[random.Next(possibleWeapons.Count)];

                            player.Points -= 100; 
                            player.Weapons.Add(chosenWeapon); 
                            player.CurrentWeapon = chosenWeapon;

                            Console.WriteLine($"You got a {chosenWeapon.Name} from the Mystery Box!");
                        }
                        else
                        {
                            Console.WriteLine("You already have all the weapons available in the Mystery Box.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough coins to buy a weapon from the Mystery Box.");
                    }
                }
            }
            else
            {
                Console.WriteLine("There's no Mystery Box in this room.");
            }
        
    }

        public void AddItemToInventory(Item item)
        {
            Items.Add(item);
        }

        public void Attack(Room room)
        {
            if (room == null || room.Zombies == null || room.Zombies.Count == 0)
            {
                Console.WriteLine("There are no zombies to attack in this room.");
                return;
            }

            var weapon = player.CurrentWeapon;
            if (weapon == null)
            {
                Console.WriteLine("You have no weapon equipped!");
                return;
            }

            var zombiesToAttack = room.Zombies.Take(weapon.AttackRange).ToList();

            foreach (var zombie in zombiesToAttack)
            {
                zombie.Health -= weapon.Damage;
                player.Points += weapon.Damage;

                Console.WriteLine($"You attacked a zombie with {weapon.Name} and dealt {weapon.Damage} damage!");

                if (zombie.Health <= 0)
                {
                    room.Zombies.Remove(zombie);
                    Console.WriteLine("The zombie was killed!");
                }
                else
                {
                    Console.WriteLine($"Zombie health remaining: {zombie.Health}");
                }
            }
        }
    




public void UseItem(Player player, string itemName)
        {
            var item = player.Items.FirstOrDefault(x => x.Name == itemName.ToLower()); 
            if (item != null)
            {
                item.Effect(player); 
                player.Items.Remove(item); 
                Console.WriteLine($"Used {item.Name}.");
            }
            else
            {
                Console.WriteLine("Item not found in inventory.");
            }
        }

        public void CheckInventory(Player player)
        {
            Console.WriteLine("Checking inventory..");
            if (player.Items == null)
            {
                Console.WriteLine("No items in inventory.");
                return;
            }

            if (player.Items.Count == 0)
            {
                Console.WriteLine("Your inventory is empty.");
            }
            else
            {
                foreach (var item in player.Items)
                {
                    Console.WriteLine($"Item: {item.Name}, Type: {item.Type}");
                }
            }
        }
        public void BuyItem(int index)
        {
            var availableItems = itemService.GetAvailableItems(); 

            if (index >= 0 && index < availableItems.Count)
            {
                var item = availableItems[index];
                int itemCost = itemService.GetItemCost(item.Name);

                if (player.Points >= itemCost) 
                {
                    player.Points -= itemCost; 
                    player.Items.Add(item); 
                    Console.WriteLine($"You bought {item.Name} for {itemCost} points.");
                    Console.WriteLine($"Items in inventory: {string.Join(", ", player.Items.Select(x => x.Name))}");
                }
                else
                {
                    Console.WriteLine($"You don't have enough points to buy {item.Name}.");
                }
            }
            else
            {
                Console.WriteLine("Invalid item selection.");
            }
        }



        public void EquipWeapon(string weaponName)
        {
            var weapon = player.Weapons.FirstOrDefault(w => w.Name.ToLower() == weaponName.ToLower());
            if (weapon != null)
            {
                player.CurrentWeapon = weapon;
                Console.WriteLine($"You have equipped {weapon.Name}.");
            }
            else
            {
                Console.WriteLine("Weapon not found!");
            }
        }
    }
}
