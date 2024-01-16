using HexedProxy.Core;
using HexedProxy.GameDumper;
using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class SaveEditor
    {
        public static void EditBloodweb(JObject Bloodweb)
        {
            if (InternalSettings.UnlockCharacters)
            {
                Bloodweb["legacyPrestigeLevel"] = 3;
                Bloodweb["prestigeLevel"] = 100;
            }

            if (InternalSettings.UnlockItems)
            {
                JArray characterItemsArray = new(
                            UE4Parser.PerkIds.Select(itemId => new JObject(
                                new JProperty("itemId", itemId),
                                new JProperty("quantity", 3)
                            ))
                            .Concat(UE4Parser.OfferingIds
                                .Concat(UE4Parser.ItemIds)
                                .Concat(UE4Parser.ItemAddonIds)
                                .Select(itemId => new JObject(
                                    new JProperty("itemId", itemId),
                                    new JProperty("quantity", 1000)
                                )))
                        );

                Bloodweb["characterItems"] = characterItemsArray;
            }
        }

        public static void EditGetAll(JObject GetAll)
        {
            JArray characterArray = GetAll["list"].ToObject<JArray>();

            if (InternalSettings.UnlockCharacters)
            {
                foreach (string CharacterId in UE4Parser.CharacterIds)
                {
                    JToken existingItem = characterArray.FirstOrDefault(item => item["characterName"]?.ToString() == CharacterId);

                    if (existingItem != null)
                    {
                        existingItem["bloodWebLevel"] = 50;
                        existingItem["isEntitled"] = true;
                        existingItem["legacyPrestigeLevel"] = 3;
                        existingItem["prestigeLevel"] = 100;
                    }
                    else
                    {
                        JObject newCharacter = new JObject(
                            new JProperty("bloodWebLevel", 50),
                            new JProperty("characterItems", new JArray()),
                            new JProperty("characterName", CharacterId),
                            new JProperty("isEntitled", true),
                            new JProperty("legacyPrestigeLevel", 3),
                            new JProperty("prestigeLevel", 100)
                        );

                        characterArray.Add(newCharacter);
                    }
                }
            }

            if (InternalSettings.UnlockItems)
            {
                foreach (string CharacterId in UE4Parser.CharacterIds)
                {
                    JToken existingItem = characterArray.FirstOrDefault(item => item["characterName"]?.ToString() == CharacterId);

                    if (existingItem != null)
                    {
                        JArray characterItemsArray = new(
                            UE4Parser.PerkIds.Select(itemId => new JObject(
                                new JProperty("itemId", itemId),
                                new JProperty("quantity", 3)
                            ))
                            .Concat(UE4Parser.OfferingIds
                                .Concat(UE4Parser.ItemIds)
                                .Concat(UE4Parser.ItemAddonIds)
                                .Select(itemId => new JObject(
                                    new JProperty("itemId", itemId),
                                    new JProperty("quantity", 1000)
                                )))
                        );

                        existingItem["characterItems"] = characterItemsArray;
                    }
                }
            }

            GetAll["list"] = characterArray;
        }

        public static void EditMarket(JObject Market)
        {
            JArray inventoryArray = Market["data"]["inventory"].ToObject<JArray>();

            if (InternalSettings.UnlockItems)
            {
                foreach (string PerkId in UE4Parser.PerkIds)
                {
                    JToken existingItem = inventoryArray.FirstOrDefault(item => item["objectId"]?.ToString() == PerkId);
                    if (existingItem != null) inventoryArray.Remove(existingItem);

                    JObject newInventoryItem = new(
                        new JProperty("lastUpdateAt", DateTimeOffset.Now.ToUnixTimeSeconds()),
                        new JProperty("objectId", PerkId),
                        new JProperty("quantity", 3)
                    );

                    inventoryArray.Add(newInventoryItem);
                }
            }

            if (InternalSettings.UnlockCosmetics)
            {
                foreach (string CosmeticId in UE4Parser.CosmeticIds.Concat(UE4Parser.OutfitIds))
                {
                    JToken existingItem = inventoryArray.FirstOrDefault(item => item["objectId"]?.ToString() == CosmeticId);
                    if (existingItem != null) inventoryArray.Remove(existingItem);

                    JObject newInventoryItem = new JObject(
                        new JProperty("lastUpdateAt", DateTimeOffset.Now.ToUnixTimeSeconds()),
                        new JProperty("objectId", CosmeticId),
                        new JProperty("quantity", 1)
                    );

                    inventoryArray.Add(newInventoryItem);
                }
            }

            Market["data"]["inventory"] = inventoryArray;
        }
    }
}
