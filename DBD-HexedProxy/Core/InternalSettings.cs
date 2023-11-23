using HexedProxy.Wrappers;
using Newtonsoft.Json;

namespace HexedProxy.Core
{
    internal class InternalSettings
    {
        public static string PlayerName;
        public static string PlayerId;
        public static string KillerId;
        public static string KillerPlatform;
        public static string KillerPlatformId;
        public static string MatchRegion;
        public static string MatchId;

        public static bool SpoofRegion = false;
        public static int TargetQueueRegion = 0;
        public static readonly string[] AvailableRegions =
        [
            "all",
            "ap-south-1",
            "eu-west-1",
            "ap-southeast-1",
            "ap-southeast-2",
            "eu-central-1",
            "ap-northeast-2",
            "ap-northeast-1",
            "us-east-1",
            "sa-east-1",
            "us-west-2"
        ];

        public static bool SpoofRank = false;
        public static int TargetRank = 1;

        public static bool SpoofOffline = false;

        public static string TargetFriendId = "";

        public static bool UnlockCosmetics = false;
        public static bool UnlockItems = false;
        public static bool UnlockLevel = false;

        public static bool InstantTomes = false;
        public static DBDObjects.ActiveNode.ResponseRoot ActiveTomeData;

        public static int SelectedGuiCategory = 0;

        public static DBDObjects.Bloodweb.ResponseRoot cachedBloodweb = JsonConvert.DeserializeObject<DBDObjects.Bloodweb.ResponseRoot>(Utils.GetFromResource("Bloodweb.json"));
        public static DBDObjects.Inventory.ResponseRoot cachedInventory = JsonConvert.DeserializeObject<DBDObjects.Inventory.ResponseRoot>(Utils.GetFromResource("Market.json"));
        public static DBDObjects.Profile.ResponseRoot cachedProfile = JsonConvert.DeserializeObject<DBDObjects.Profile.ResponseRoot>(Utils.GetFromResource("GetAll.json"));
    }
}
