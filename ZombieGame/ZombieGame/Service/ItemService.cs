using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieGame.Models;

namespace ZombieGame.Service
{
    public class ItemService
    {
        private Dictionary<string, Item> items;
        private Dictionary<string, int> itemCosts;

        public ItemService() {
            InitializeItems();

        }

        public void InitializeItems()
        {
            const int maxHealth = 150; // Maximum health limit for the game

            // Initiera dictionary för objekt
            items = new Dictionary<string, Item>
    {
        {
            "Health Potion", new Item
            {
                Name = "Health Potion",
                Type = "Healing",
                Effect = player =>
                {
                    player.Health += 50;
                    if (player.Health > 100)
                    {
                        player.Health = 100; // Health Potion should not exceed 100
                    }
                    Console.WriteLine($"Used Health Potion. Current health: {player.Health}");
                }
            }
        },
        {
            "Juggernaut", new Item
            {
                Name = "Juggernaut",
                Type = "Boost",
                Effect = player =>
                {
                    if (player.Health < maxHealth)
                    {
                        player.Health += 50; // Juggernaut increases health but with a cap
                        if (player.Health > maxHealth)
                        {
                            player.Health = maxHealth; // Cap health at 150
                        }
                        Console.WriteLine($"Used Juggernaut. Health is now {player.Health}");
                    }
                    else
                    {
                        Console.WriteLine("Health is already at its maximum.");
                    }
                }
            }
        },
        {
            "Self Revive", new Item
            {
                Name = "Self Revive",
                Type = "Revival",
                Effect = player =>
                {
                    if (player.isDown)
                    {
                        player.isDown = false;
                        player.Health = 50; // When player is revived, start with 50 health
                        Console.WriteLine("Used Self Revive to revive yourself.");
                    }
                    else
                    {
                        Console.WriteLine("Self Revive has no effect because you're not down.");
                    }
                }
            }
        }
    };

           
            itemCosts = new Dictionary<string, int>
    {
        { "Health Potion", 100 },
        { "Juggernaut", 100 },    
        { "Self Revive", 100 }    
    };
        }



        public Item GetItem(string itemName)
        {
            if(items.TryGetValue(itemName, out Item item))
            {
                return item;
            }
            return null;
        }
        public bool HasItem (Player player, string itemName)
        {
            return player.Items.Any(i => i.Name == itemName);
        }
        public void UseItem(Player player, string itemName)
        {
            var item = GetItem(itemName);
            if (item != null && player.Items.Contains(item))
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
        public List<Item> GetAvailableItems()
        {
            return new List<Item>(items.Values); 
        }
        public int GetItemCost (string itemName)
        {
            if (itemCosts.ContainsKey(itemName)){
                return itemCosts[itemName];
            }
            else
            {
                throw new Exception($"Item cost not found for '{itemName}'");
            }
        }
    }
}

