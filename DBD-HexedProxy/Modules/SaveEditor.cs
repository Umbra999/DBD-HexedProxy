﻿using HexedProxy.Wrappers;
using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class SaveEditor
    {
        private static JObject cachedBloodweb = JObject.Parse(Utils.GetFromResource("Bloodweb.json"));
        private static JObject cachedInventory = JObject.Parse(Utils.GetFromResource("Market.json"));
        private static JObject cachedProfile = JObject.Parse(Utils.GetFromResource("GetAll.json"));

        public static int PrestigeLevel = 100;
        public static int LegacyPrestigeLevel = 3;

        public static void EditBloodweb(JObject Bloodweb)
        {
            Bloodweb["legacyPrestigeLevel"] = LegacyPrestigeLevel;
            Bloodweb["prestigeLevel"] = PrestigeLevel;

            // add character data if needed
        }

        public static void EditGetAll(JObject GetAll)
        {
            GetAll["list"] = cachedProfile["list"];

            foreach (var list in GetAll["list"])
            {
                list["prestigeLevel"] = PrestigeLevel;
                list["legacyPrestigeLevel"] = LegacyPrestigeLevel;
            }
        }

        public static void EditMarket(JObject Market)
        {
            Market["data"]["inventory"] = cachedInventory["data"]["inventory"];
        }

        public static void EditCurrencies(JObject Currencies)
        {
            foreach (var Currency in Currencies["list"])
            {
                Currency["balance"] = 9999999;
            }
        }
    }
}
