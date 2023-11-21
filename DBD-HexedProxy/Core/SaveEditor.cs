using HexedProxy.Wrappers;
using Newtonsoft.Json;

namespace HexedProxy.Core
{
    internal class SaveEditor
    {
        public static DBDObjects.DBDBloodweb cachedBloodweb;
        public static DBDObjects.DBDInventory cachedInventory;
        public static DBDObjects.DBDProfile cachedProfile;

        public static void Init()
        {
            EditBloodweb();
            EditInventory();
            EditProfile();
        }

        private static void EditBloodweb()
        {
            DBDObjects.DBDBloodweb PlayerBloodweb = JsonConvert.DeserializeObject<DBDObjects.DBDBloodweb>(Utils.GetFromResource("Bloodweb.json"));

            PlayerBloodweb.prestigeLevel = 100;
            PlayerBloodweb.legacyPrestigeLevel = 3;
            PlayerBloodweb.bloodWebLevel = 50;

            cachedBloodweb = PlayerBloodweb;
        }

        private static void EditInventory()
        {
            DBDObjects.DBDInventory PlayerInventory = JsonConvert.DeserializeObject<DBDObjects.DBDInventory>(Utils.GetFromResource("Market.json"));

            PlayerInventory.data.playerId = "";

            cachedInventory = PlayerInventory;
        }

        private static void EditProfile()
        {
            DBDObjects.DBDProfile PlayerProfile = JsonConvert.DeserializeObject<DBDObjects.DBDProfile>(Utils.GetFromResource("GetAll.json"));

            foreach (DBDObjects.Character Char in PlayerProfile.list)
            {
                Char.prestigeLevel = 100;
                Char.legacyPrestigeLevel = 3;
                Char.bloodWebLevel = 50;

                foreach (DBDObjects.Characteritem Item in Char.characterItems)
                {
                    Item.quantity = 999;
                }
            }

            cachedProfile = PlayerProfile;
        }
    }
}
