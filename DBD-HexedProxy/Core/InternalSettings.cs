namespace HexedProxy.Core
{
    internal class InternalSettings
    {
        public static string PlayerName = "NOT NAME";
        public static string PlayerId = "NOT ID";
        public static string KillerId = "NO KILLER";
        public static string MatchRegion = "NO REGION";
        public static string MatchId = "NO MATCH";

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

        public static string AddFriendId = "";

        public static bool UnlockCosmetics = false;
        public static bool UnlockItems = false;
        public static bool UnlockLevel = false;

        public static int SelectedGuiCategory = 0;
    }
}
