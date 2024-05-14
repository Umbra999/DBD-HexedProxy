using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Versions;
using HexedProxy.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace HexedProxy.GameDumper
{
    internal class UE4Parser
    {
        public static string LastKnownDirectoryPath;
        private static DefaultFileProvider Provider;

        public static void Initialize()
        {
            if (LastKnownDirectoryPath == null) return;

            string paksPath = LastKnownDirectoryPath + "DeadByDaylight\\Content\\Paks";

            Provider = new DefaultFileProvider(paksPath, SearchOption.AllDirectories, true, new VersionContainer((EGame)536870945, 0, default, null, null, null));
            Provider.Initialize();
            Provider.Mount();

            DumpDatabase();
        }

        private static string GetAccessKey()
        {
            Provider.TrySaveAsset("DeadByDaylight/Config/DefaultGame.ini", out byte[] data);

            string key = "";

            using (MemoryStream memoryStream = new(data))
            {
                using StreamReader streamReader = new(memoryStream);
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.Contains("_live")) key = line;
                }
            }

            return Regex.Match(key, "Key=\"(.*?)\"").Groups[1].Value;
        }

        private static void DumpDatabase()
        {
            CharacterIds.Clear();
            OfferingIds.Clear();
            ItemIds.Clear();
            ItemAddonIds.Clear();
            OutfitIds.Clear();
            CosmeticIds.Clear();
            PerkIds.Clear();

            foreach (KeyValuePair<string, GameFile> keyValuePair in Provider.Files.Where((KeyValuePair<string, GameFile> val) => val.Value.Path.Contains("DeadByDaylight/Content/Data")))
            {
                string name = keyValuePair.Value.Name;

                switch (name)
                {
                    case "CharacterDescriptionDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "CharacterId");

                            foreach (object id in foundIds) 
                            {
                                if (!CharacterIds.Contains(id.ToString())) CharacterIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "OfferingDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "ItemId");

                            foreach (object id in foundIds)
                            {
                                if (!OfferingIds.Contains(id.ToString())) OfferingIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "ItemDB.uasset": // needs check if its a power or a item thats killer based like lawment box
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "ItemId");

                            foreach (object id in foundIds)
                            {
                                if (!ItemIds.Contains(id.ToString())) ItemIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "ItemAddonDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "ItemId");

                            foreach (object id in foundIds)
                            {
                                if (!ItemAddonIds.Contains(id.ToString())) ItemAddonIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "OutfitDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "ID");

                            foreach (object id in foundIds)
                            {
                                if (!OutfitIds.Contains(id.ToString())) OutfitIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "CustomizationItemDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "CustomizationId");

                            foreach (object id in foundIds)
                            {
                                if (!CosmeticIds.Contains(id.ToString())) CosmeticIds.Add(id.ToString());
                            }
                        }
                        break;

                    case "PerkDB.uasset":
                        {
                            IPackage package = Provider.LoadPackage(keyValuePair.Value.Path);

                            string packageJson = JsonConvert.SerializeObject(package.GetExports());

                            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(packageJson);

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "ItemId");

                            foreach (object id in foundIds)
                            {
                                if (!PerkIds.Contains(id.ToString())) PerkIds.Add(id.ToString());
                            }
                        }
                        break;
                }
            }
        }

        public static List<string> CharacterIds = new();
        public static List<string> OfferingIds = new();
        public static List<string> ItemIds = new();
        public static List<string> ItemAddonIds = new();
        public static List<string> OutfitIds = new();
        public static List<string> CosmeticIds = new();
        public static List<string> PerkIds = new();
    }
}
