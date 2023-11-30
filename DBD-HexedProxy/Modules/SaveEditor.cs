using HexedProxy.Wrappers;
using Newtonsoft.Json;

namespace HexedProxy.Modules
{
    internal class SaveEditor
    {
        private static DBDObjects.Bloodweb.ResponseRoot cachedBloodweb = JsonConvert.DeserializeObject<DBDObjects.Bloodweb.ResponseRoot>(Utils.GetFromResource("Bloodweb.json"));
        private static DBDObjects.Inventory.ResponseRoot cachedInventory = JsonConvert.DeserializeObject<DBDObjects.Inventory.ResponseRoot>(Utils.GetFromResource("Market.json"));
        private static DBDObjects.Profile.ResponseRoot cachedProfile = JsonConvert.DeserializeObject<DBDObjects.Profile.ResponseRoot>(Utils.GetFromResource("GetAll.json"));

        public static int PrestigeLevel = 100;
        public static int LegacyPrestigeLevel = 3;
        public static int BloodwebLevel = 49;

        public static void EditBloodweb(DBDObjects.Bloodweb.ResponseRoot Bloodweb)
        {
            Bloodweb.legacyPrestigeLevel = LegacyPrestigeLevel;
            Bloodweb.prestigeLevel = PrestigeLevel;

            // add character data if needed
        }

        public static void EditGetAll(DBDObjects.Profile.ResponseRoot GetAll)
        {
            GetAll.list = cachedProfile.list;

            foreach (var list in GetAll.list)
            {
                list.prestigeLevel = PrestigeLevel;
                list.legacyPrestigeLevel= LegacyPrestigeLevel;
            }
        }

        public static void EditMarket(DBDObjects.Inventory.ResponseRoot Market)
        {
            Market.data.inventory = cachedInventory.data.inventory;
        }
    }
}
