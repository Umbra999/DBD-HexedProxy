using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Versions;
using HexedProxy.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            Provider = new DefaultFileProvider(paksPath, SearchOption.AllDirectories, false, new VersionContainer(EGame.GAME_DeadbyDaylight));
            Provider.Initialize();

            Provider.SubmitKey(default, new FAesKey("0x22B1639B548124925CF7B9CBAA09F9AC295FCF0324586D6B37EE1D42670B39B3"));
            Provider.LoadLocalization(ELanguage.English);
            Provider.MappingsContainer = new FileUsmapTypeMappingsProvider("DBDMap.usmap");

            DumpDatabase();
        }

        private static void DumpDatabase()
        {
            CharacterIds.Clear();
            OfferingIds.Clear();
            ItemIds.Clear();
            ItemAddonIds.Clear();
            OutfitIds.Clear();
            PerkIds.Clear();

            foreach (KeyValuePair<string, GameFile> keyValuePair in Provider.Files)
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

                            List<object> foundIds = Utils.FindJsonFields(jsonArray, "Id");

                            foreach (object id in foundIds)
                            {
                                if (!OutfitIds.Contains(id.ToString())) OutfitIds.Add(id.ToString());
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
        public static List<string> PerkIds = new();
    }
}
