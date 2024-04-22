using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieGame.Helpers;
using ZombieGame.Models;
using ZombieGame.Settings;

namespace ZombieGame.Service
{
    public class GameService
    {
        private GameState gameState;
        private PlayerService playerService;
        private RoomService roomService;
        private ZombieService zombieService;
        private ItemService itemService;
        private WeaponService weaponService;
        public GameService(GameState gamestate, PlayerService playerservice, RoomService roomservice, ZombieService zombieservice, ItemService itemservice, WeaponService weaponservice) 
        {
            gameState = gamestate;
            playerService = playerservice;
            roomService = roomservice;
            zombieService = zombieservice;
            itemService = itemservice;
            weaponService = weaponservice;



        }

        public void StartGame()
        {
            Console.WriteLine("Do you want to load the previous game? (yes/no)");
            string answer = Console.ReadLine().Trim().ToLower();

            if (answer == "yes")
            {
                GameState loadedState = GameHelper.LoadGame();
                if (loadedState != null && !loadedState.IsGameOver)
                {
                    gameState = loadedState;
                    Console.WriteLine($"Game loaded successfully. Welcome back, {gameState.Player.Name}.");
                    
                }
                else
                {
                    Console.WriteLine("Cannot load the game or the game is over. Starting a new game.");
                    StartNewGame(); 
                }
            }
            else if (answer == "no")
            {
                StartNewGame(); 
            }
            else
            {
                Console.WriteLine("Invalid response. Please answer 'yes' or 'no'.");
                StartGame();
            }

            GameLoop(); 
        }

        public void StartNewGame()
        {
            Introduction();
            IninitializePlayer();
            roomService.AssignZombiesToAllRooms(); 
            gameState.TotalZombies = roomService.Rooms.Sum(room => room.Zombies.Count);
            Console.WriteLine($"Game started with a total of {gameState.TotalZombies} zombies.");
        }



        public void Initialize()
        {
            roomService.CreateRooms();
            gameState.CurrentRoom = roomService.GetCurrentRoom();
            roomService.AssignZombiesToAllRooms();

          
            foreach (var room in roomService.Rooms)
            {
                Console.WriteLine($"Room: {room.Name}, Zombies: {room.Zombies?.Count ?? 0}");
            }

            gameState.TotalZombies = roomService.Rooms.Sum(room => room.Zombies?.Count ?? 0);
        }


        private void Introduction()
        {
            Console.WriteLine("Welcome to PAAM of Duty. The year is 2130..");
            Console.WriteLine("Former PAAM employees have turned into zombies eating some runkbullar");
            Console.WriteLine("You must stop this spread and kill them all..");
            Console.WriteLine("Survive by earning money from killing zombies or (y solving some puzzles)");
            Console.WriteLine("Use your earnings to buy weapons and (unlock new areas of the map to explore).");
            Console.WriteLine("Type 'help' at any time for instructions on how to play the game.");


        }
        public void IninitializePlayer()
        {
            Console.WriteLine("Please enter your name:");
            var playerName = Console.ReadLine();

    
            Player newPlayer = playerService.CreateNewPlayer(playerName);

            if (newPlayer == null)
            {
                throw new Exception("Failed to create a new player. The player is null.");
            }

            gameState = new GameState(); 
            gameState.Player = newPlayer;

          
            gameState.CurrentRoom = roomService.GetCurrentRoom();
            newPlayer.CurrentRoom = gameState.CurrentRoom;

            Console.WriteLine($"Welcome, {gameState.Player.Name}! Your survival journey begins now.");
        }

        //The main loop for the game
        public void GameLoop()
        {
            while (!gameState.IsGameOver)
            {
                ShowMenu(); 
                ProcessInput();  

               
                if (new Random().Next(100) < 20) // 20% chance for zombieattack
                {
                    zombieService.ZombieAttack(gameState.Player, gameState.CurrentRoom);
                    if (gameState.Player.Health <= 0)
                    {
                        Console.WriteLine("You have died. Game over.");
                        gameState.IsGameOver = true;  
                    }
                }
            }
            Console.WriteLine("Thank you for playing!"); 
        }



        public void ShowMenu()
        {
            Console.WriteLine("\n----- Zombie Game Menu -----");

            if (gameState.CurrentRoom != null)
            {
                Console.WriteLine($"You are in the {gameState.CurrentRoom.Name}.");
                Console.WriteLine($"Your points: {gameState.Player.Points}");
                Console.WriteLine($"Your health: {gameState.Player.Health}");
                Console.WriteLine($"Zombies in this room: {gameState.CurrentRoom.Zombies.Count}");  

                if (gameState.TotalZombies >= 0)  
                {
                    Console.WriteLine($"Total zombies remaining: {gameState.TotalZombies}");  
                }
            }
            else
            {
                Console.WriteLine("Error: No current room available.");
            }

            Console.WriteLine("Available commands:");
            Console.WriteLine("1. Move");
            Console.WriteLine("2. Attack");
            Console.WriteLine("3. Run");
            Console.WriteLine("4. Check Inventory");
            Console.WriteLine("5. Use Item");
            Console.WriteLine("6. Buy Item");
            Console.WriteLine("7. Save Game");
            Console.WriteLine("8. Buy from the Mystery Box");
            Console.WriteLine("9. Exit Game");
            Console.WriteLine("---------------------------");
            Console.Write("Enter your choice: ");
        }

        public void CheckPlayerInventory()
        {
            Console.WriteLine("Checking inventory...");

            
            playerService.CheckInventory(gameState.Player);  
        }




        public void ProcessInput()
        {
            string input = Console.ReadLine().ToLower();
            switch (input)
            {
                case "1":
                case "move":
                    ShowMoveMenu();
                    break;
                case "2":
                case "attack":
                    AttackSequence();
                    gameState.Player.Points = playerService.player.Points;
                    break;
                case "3":
                case "run":
                    RunAway();
                    break;
                case "4":
                case "check inventory":
                    CheckPlayerInventory();
                    break;
                case "5":
                case "use item":
                    UseItem(gameState);
                    break;
                case "6":
                case "buy item":
                    //ShowAvailableItems();
                    BuyItem();
                    break;
                case "7":
                case "save":
                    SaveGame();
                    break;
                case "8":
                case "buy from the mysterbox":
                case "mysterbox":
                    InteractWithMysteryBox();
                    break;
                case "9":
                case "exit":
                    Console.WriteLine("Exiting game...");
                    gameState.IsGameOver = true;
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid command. Please try again.");
                    break;
            }
        }
        private void TryRevivePlayer()
        {
            if (itemService.HasItem(gameState.Player, "Self Revive"))  
            {
                playerService.Revive();  
                Console.WriteLine("Player has been revived using Self Revive.");
            }
            else
            {
                gameState.IsGameOver = true;  
                Console.WriteLine("Player has died. Game over.");
                GameHelper.DeleteSaveFile();  
            }
        }

        private void ShowMoveMenu()
        {
            Console.WriteLine("Which direction? (north, south, east, west)");
            Console.WriteLine("Available directions:");
            if (gameState.CurrentRoom.North != null) Console.WriteLine("- North");
            if (gameState.CurrentRoom.South != null) Console.WriteLine("- South");
            if (gameState.CurrentRoom.East != null) Console.WriteLine("- East");
            if (gameState.CurrentRoom.West != null) Console.WriteLine("- West");
            Console.Write("Enter your choice: ");

            string direction = Console.ReadLine().ToLower();
            MovePlayer(direction);
        }
        private void MovePlayer(string direction)
        {
            Room nextRoom = roomService.Move(gameState.Player, direction);
            if (nextRoom != null)
            {
                Console.WriteLine($"You moved to {nextRoom.Name}.");
                gameState.CurrentRoom = nextRoom;
                
            }
            else
            {
                Console.WriteLine("You can't move in that direction.");
            }
        }
        public void AttackSequence()
        {
            var weapon = gameState.Player.CurrentWeapon;

            if (weapon == null)
            {
                Console.WriteLine("You have no weapon equipped!");
                return;
            }

            Console.WriteLine("Press 'Enter' to attack. Type 'stop' to end the attack sequence.");

            while (true)
            {
                var zombiesInRoom = gameState.CurrentRoom.Zombies.Where(z => z.Health > 0).ToList();

                if (zombiesInRoom.Count == 0)
                {
                    Console.WriteLine("There are no zombies to attack in this room.");
                    break;
                }

                string input = Console.ReadLine().ToLower();

                if (input == "stop")
                {
                    Console.WriteLine("Attack sequence ended.");
                    break;
                }

                Random random = new Random();
                bool miss = random.Next(100) < 20; 
                if (miss)
                {
                    Console.WriteLine("You missed!");
                    continue;
                }

                var targetZombie = zombiesInRoom.First();
                playerService.Attack(gameState.CurrentRoom);

                if (targetZombie.Health <= 0)
                {
                    gameState.CurrentRoom.Zombies.Remove(targetZombie);
                    gameState.TotalZombies--;
                    Console.WriteLine("The zombie has been killed!");
                }

                bool zombieAttacks = random.Next(100) < 30;
                if (zombieAttacks)
                {
                    zombieService.ZombieAttack(gameState.Player, gameState.CurrentRoom);
                    if (gameState.Player.Health <= 0)
                    {
                        TryRevivePlayer();  
                        if (gameState.IsGameOver)  
                        {
                            Console.WriteLine("Game over.");
                            break;
                        }
                    }
                }
            }
        }







        private void ShowAvailableItems()
        {
            Console.WriteLine("Available items to buy:");
            var availableItems = itemService.GetAvailableItems();

            for (int i = 0; i < availableItems.Count; i++)
            {
                var item = availableItems[i];
                
                int cost = itemService.GetItemCost(item.Name);
                Console.WriteLine($"{i + 1}: {item.Name} - {item.Type}, Cost: {cost} points");
            }
        }


        public void RunAway()
        {
            
            Console.WriteLine("You ran away safely to another room...");
            
        }


        public void UseItem(GameState gameState)
        {
            var player = gameState.Player;

            
            if (player.Items == null || player.Items.Count == 0)
            {
                Console.WriteLine("Your inventory is empty.");
                return;
            }

            Console.WriteLine("Select an item to use:");

           
            for (int i = 0; i < player.Items.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {player.Items[i].Name} (Type: {player.Items[i].Type})");
            }

            Console.Write("Enter the number or name of the item to use: "); 
            var input = Console.ReadLine().Trim().ToLower();  

            Item selectedItem = null;

            if (int.TryParse(input, out int index))  
            {
                if (index >= 1 && index <= player.Items.Count)
                {
                    selectedItem = player.Items[index - 1];  
                }
                else
                {
                    Console.WriteLine("Invalid selection.");  
                    return;
                }
            }
            else  
            {
                selectedItem = player.Items.FirstOrDefault(x => x.Name.ToLower() == input);
            }

            if (selectedItem != null)  
            {
                selectedItem.Effect(player);  
                player.Items.Remove(selectedItem); 
                Console.WriteLine($"Used {selectedItem.Name}.");
            }
            else
            {
                Console.WriteLine("Item not found in inventory.");  
            }
        }

        public void SaveGame()
        {
            GameHelper.SaveGame(gameState);
            Console.WriteLine("Game has been successfully saved.");
        }
        public void InteractWithMysteryBox()
        {
            if (gameState.CurrentRoom.Name == "Lounge")
            {
                Console.WriteLine("Welcome to the Mystery Box! Would you like to buy a random weapon for 100 points? (yes/no)");
                var answer = Console.ReadLine().Trim().ToLower();

                if (answer == "yes")
                {
                    if (gameState.Player.Points >= 100)
                    {
                        var currentWeaponNames = gameState.Player.Weapons?.Select(w => w.Name).ToList() ?? new List<string>();
                        var possibleWeapons = weaponService.GetAllWeapons()
                            .Where(w => !currentWeaponNames.Contains(w.Name))  
                            .ToList();

                        if (possibleWeapons.Any())
                        {
                            var random = new Random();
                            var chosenWeapon = possibleWeapons[random.Next(possibleWeapons.Count)];

                            gameState.Player.Points -= 100;  

                            // Ta bort Colt om den finns
                            var startWeapon = gameState.Player.Weapons.FirstOrDefault(w => w.Name == "16 Colt M1911");
                            if (startWeapon != null)
                            {
                                gameState.Player.Weapons.Remove(startWeapon);  
                            }

                            gameState.Player.Weapons.Add(chosenWeapon); 
                            gameState.Player.CurrentWeapon = chosenWeapon;  

                            Console.WriteLine($"You got a {chosenWeapon.Name} from the Mystery Box!");
                        }
                        else
                        {
                            Console.WriteLine("You already have all the weapons from the Mystery Box.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough points to buy a weapon from the Mystery Box.");
                    }
                }
                else
                {
                    Console.WriteLine("No weapon was bought from the Mystery Box.");
                }
            }
            else
            {
                Console.WriteLine("There's no Mystery Box in this room.");
            }
        }




        public void BuyItem()
        {
            Console.WriteLine("Available items:");
            var availableItems = itemService.GetAvailableItems(); 

            for (int i = 0; i < availableItems.Count; i++)
            {
                var item = availableItems[i];
                int cost = itemService.GetItemCost(item.Name);
                Console.WriteLine($"{i + 1}: {availableItems[i].Name} - {availableItems[i].Type}, Cost: {cost} points");
            }

            Console.WriteLine("Enter the number of the item you want to buy:");
            string response = Console.ReadLine();

           
            if (int.TryParse(response, out int index))
            {
               
                if (index >= 1 && index <= availableItems.Count) 
                {
                    var selectedItem = availableItems[index - 1]; 
                    int itemCost = itemService.GetItemCost(selectedItem.Name);

                    if (gameState.Player.Points >= itemCost)  
                    {
                        gameState.Player.Points -= itemCost;  
                        gameState.Player.Items.Add(selectedItem);  
                        Console.WriteLine($"You bought {selectedItem.Name}. Remaining points: {gameState.Player.Points}");
                    }
                    else
                    {
                        Console.WriteLine("You don't have enough points to buy this item.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid item selection."); 
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");  
            }
        }

        private void LoadGame()
        {
            GameState loadedState = GameHelper.LoadGame();  
            if (loadedState != null && !loadedState.IsGameOver)
            {
                gameState = loadedState;
                Console.WriteLine($"Game loaded successfully. Welcome back {gameState.Player.Name}");
                Console.WriteLine("Continuing from last save point.");
            }
            else
            {
                Console.WriteLine("Failed to load game or the game is over. Starting a new game.");
                Introduction();
                IninitializePlayer();
            }
        }
    }


    
}
