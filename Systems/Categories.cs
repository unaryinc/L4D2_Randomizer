using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Unary.L4D2_Randomizer.Abstract;

namespace Unary.L4D2_Randomizer.Systems
{
    public class Categories : ISystem
    {
        private Dictionary<string, List<string>> Entries;
        public Dictionary<string, Func<List<string>>> Processors;
        private Dictionary<string, Queue<string>> QueueudEntries;

        private GameType Type;
        private Random Random;

        public override void Init()
        {
            Entries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(File.ReadAllText("Groups.json"));
            Processors = new Dictionary<string, Func<List<string>>>();
            QueueudEntries = new Dictionary<string, Queue<string>>();

            Type = Sys.Ref.Get<GameType>();
            Random = Sys.Ref.Get<Random>();

            Processors["player"] = OnPlayer;
            Processors["weapon"] = OnWeapon;
            Processors["melee"] = OnMelee;
            Processors["entity"] = OnEntity;
            Processors["item"] = OnItem;
            Processors["upgrade"] = OnUpgrade;
            Processors["zombie"] = OnZombie;
            Processors["explosive"] = OnExplosive;
            Processors["equipable"] = OnEquipable;
            Processors["ammomodel"] = OnAmmoModel;
            Processors["staticmodel"] = OnStaticModel;
            Processors["physmodel"] = OnPhysModel;
            Processors["ragdoll"] = OnRagdoll;
            Processors["doormodel"] = OnDoorModel;
            Processors["dummymodel"] = OnDummyModel;

            foreach(var Category in Processors)
            {
                QueueudEntries[Category.Key] = new Queue<string>();
            }
        }

        public string GetRandomEntry(string Name)
        {
            if(QueueudEntries[Name].Count == 0)
            {
                List<string> Pool = Processors[Name].Invoke();

                foreach (var Entry in Pool.OrderBy(x => Random.Next()).ToList())
                {
                    QueueudEntries[Name].Enqueue(Entry);
                }
            }

            return QueueudEntries[Name].Dequeue();
        }
        
        private List<string> OnPlayer()
        {
            List<string> Result = new List<string>();
            Result.AddRange(Entries["L4D2"]);
            Result.AddRange(Entries["L4D"]);
            return Result;
        }

        private List<string> OnWeapon()
        {
            List<string> Result = new List<string>();
            Result.AddRange(Entries["Melee"]);
            Result.AddRange(Entries["Fire"]);
            return Result;
        }

        private List<string> OnMelee()
        {
            return Entries["Melee"];
        }

        private List<string> OnEntity()
        {
            List<string> Result = new List<string>();
            Result.AddRange(Entries["Zombies"]);
            Result.AddRange(Entries["L4D2"]);
            Result.AddRange(Entries["L4D"]);
            return Result;
        }

        private List<string> OnItem()
        {
            return Entries["Items"];
        }

        private List<string> OnUpgrade()
        {
            return Entries["Upgrades"];
        }

        private List<string> OnZombie()
        {
            return Entries["Zombies"];
        }

        private List<string> OnExplosive()
        {
            return Entries["Explosives"];
        }

        private List<string> OnEquipable()
        {
            return Entries["Equipables"];
        }

        private List<string> OnAmmoModel()
        {
            return Entries["AmmoModels"];
        }

        private List<string> OnStaticModel()
        {
            return Entries["StaticModels"];
        }

        private List<string> OnPhysModel()
        {
            return Entries["PhysModels"];
        }

        private List<string> OnRagdoll()
        {
            return Entries["Ragdols"];
        }

        private List<string> OnDoorModel()
        {
            return Entries["DoorModels"];
        }

        private List<string> OnDummyModel()
        {
            return Entries["DummyModels"];
        }
    }
}
