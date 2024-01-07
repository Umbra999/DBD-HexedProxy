namespace HexedProxy.Core
{
    internal class InternalSettings
    {
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

        public static bool InstantTomes = false;
        public static bool BlockTomes = false;

        public static string TargetFriendId = "";

        public static bool MatchSnipe = false;
        public static bool OnlyStreamer = false;
        public static string TargetSnipeParameter = "";

        public static bool NameSpoof = false;
        public static string TargetCustomName = "";

        public static bool UnlockAll = false;

        public static bool DisableKillswitch = false;   

        public static int SelectedGuiCategory = 0;
    }
}
