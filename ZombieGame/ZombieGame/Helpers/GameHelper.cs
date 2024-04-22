using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ZombieGame.Settings;

namespace ZombieGame.Helpers
{
    public class GameHelper
    {
        private static string SaveFilePath = "gamestate.json";

        public static void SaveGame(GameState gameState)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            try
            {
                string jsonString = JsonSerializer.Serialize(gameState, options);
                File.WriteAllText(SaveFilePath, jsonString);
                Console.WriteLine("Game has been saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save the game: {ex.Message}");
            }
        }

        public static GameState LoadGame()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve 

            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string jsonString = File.ReadAllText(SaveFilePath);
                    GameState gameState = JsonSerializer.Deserialize<GameState>(jsonString, options);
                    if (gameState.IsGameOver || (gameState.Player != null && gameState.Player.isDown))
                    {
                        Console.WriteLine("Cannot load the game as it is over or the player is downed. Please start a new game.");
                        return null;                     }

                    Console.WriteLine("Game loaded successfully.");
                    return gameState;
                }
                else
                {
                    Console.WriteLine("No saved game found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load the game: {ex.Message}");
                return null;
            }
        
            
        }
        public static void DeleteSaveFile() { 
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath); 
                    Console.WriteLine("Game save file has been deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete the game save file: {ex.Message}");
            }
        }
    }
}

    
