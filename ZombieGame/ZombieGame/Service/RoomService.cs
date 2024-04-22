
using ZombieGame.Models;

namespace ZombieGame.Service
{
    public class RoomService { 

        public Room currentRoom { get; set; }
        public List<Room>Rooms { get; set; }
        public bool zombiesGenerated { get; private set; } = false;
        public RoomService()
        {
            CreateRooms();
           
        }
        public void CreateRooms()
        {
            Room hallway = new Room { Name = "Hallway", Zombies = new List<Zombie>(), hasMysteryBox = false };
            Room lounge = new Room { Name = "Lounge", Zombies = new List<Zombie>(), hasMysteryBox = true};
            Room kitchen = new Room { Name = "Kitchen", Zombies = new List<Zombie>(), hasMysteryBox = false};
            Room diningRoom = new Room { Name = "Dining Room", Zombies = new List<Zombie>(), hasMysteryBox = true };
            Room pinballRoom = new Room { Name = "Pinball Room", Zombies = new List<Zombie>(), hasMysteryBox = true };
            Room office = new Room { Name = "Office", Zombies = new List<Zombie>(), hasMysteryBox = true };

            // Setup direct connections
            hallway.North = lounge;
            hallway.West = office;
            office.East = hallway;
            lounge.South = hallway;
            lounge.West = kitchen;
            kitchen.North = diningRoom;
            kitchen.East = lounge;
            lounge.East = pinballRoom;
            pinballRoom.West = lounge;
            diningRoom.South = kitchen;


            Rooms = new List<Room> { hallway, lounge, kitchen, diningRoom, pinballRoom, office };
          
            currentRoom = hallway;

        }
        public void AssignZombiesToAllRooms()
        {
            foreach (var room in Rooms)
            {
                if (!room.ZombiesGenerated) 
                {
                    AssignZombiesToRoom(room);
                    room.ZombiesGenerated = true; 
                }
            }
        }

        private void AssignZombiesToRoom(Room room)
        {
            var random = new Random();
            int zombieCount = random.Next(5, 15); 

            room.Zombies = new List<Zombie>(); 

            for (int i = 0; i < zombieCount; i++)
            {
                room.Zombies.Add(new Zombie { Health = 100 });
            }
        }


        public Room GetCurrentRoom()
        {
            return currentRoom;
        }
        public Room Move (Player player, string direction)
        {
            try
            {
                Room nextRoom = null;


                switch (direction.ToLower())
                {
                    case "north":
                        nextRoom = player.CurrentRoom.North;
                        break;
                    case "south":
                        nextRoom = player.CurrentRoom.South;
                        break;
                    case "east":
                        nextRoom = player.CurrentRoom.East;
                        break;
                    case "west":
                        nextRoom = player.CurrentRoom.West;
                        break;


                }
                if (currentRoom != null)
                {
                    player.CurrentRoom = nextRoom;
                    return nextRoom;
                }
                else
                {
                    Console.WriteLine("There's no room in that direction.");
                    return player.CurrentRoom;
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
