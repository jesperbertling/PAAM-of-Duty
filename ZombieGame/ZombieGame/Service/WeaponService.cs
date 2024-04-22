using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZombieGame.Models;

namespace ZombieGame.Service
{
    public class WeaponService
    {
        public List<Weapon> Weapons;

        public WeaponService() {
            Weapons = new List<Weapon>();
        }
        public void InitializeWeapon(Player player)
        {
            if (Weapons == null || !Weapons.Any())  
            {
                Weapons = new List<Weapon>
        {
            new Weapon { Name = "16 Colt M1911", Type = "Pistol", Damage = 100, AttackRange = 1 },
            new Weapon { Name = "Raygun", Type = "Wonder weapon", Damage = 200, AttackRange = 5 },
            new Weapon { Name = "Combat knife", Type = "Knife", Damage = 50, AttackRange = 1 },
            new Weapon { Name = "SPAS-12", Type = "Shotgun", Damage = 75, AttackRange = 3 }
        };
            }

            if (player.Weapons == null)  
            {
                player.Weapons = new List<Weapon>();
            }

            if (!player.Weapons.Any())  
            {
                var startWeapon = Weapons[0];  
                player.Weapons.Add(startWeapon);  
                player.CurrentWeapon = startWeapon;  
            }
        }


        public List<Weapon> GetAllWeapons()
        {
            return Weapons;
        }
        
        public string GetWeaponInfo(string weaponName)
        {
            var weapon = Weapons.FirstOrDefault(w => w.Name.ToLower() == weaponName);
            if (weapon!=null){
                return $"This weapon {weapon.Name} is a {weapon.Type}, damage: {weapon.Damage}";

            }
            return "Weapon not found";
        }
        public Weapon GetFirstWeapon()
        {
            return Weapons[0]; //Start vapen
        }
        public Weapon GetRandomWeapon()
        {
            Random random = new Random();
            int index = random.Next(Weapons.Count); //Randomize the weapon
            return Weapons[index]; 
        }

    }
}
