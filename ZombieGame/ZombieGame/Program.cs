﻿using ZombieGame.Service;
using ZombieGame.Settings;

class Program
{
    static void  Main(string[] args)
    {

        RoomService roomService = new RoomService();
        ItemService itemService = new ItemService();
        GameState gameState = new GameState();
        WeaponService weaponService = new WeaponService();
        PlayerService playerService = new PlayerService(weaponService, roomService);
        ZombieService zombieService = new ZombieService(playerService);

       
        GameService gameService = new GameService(gameState, playerService, roomService, zombieService, itemService, weaponService);

        
        gameService.StartGame();

    }
}
