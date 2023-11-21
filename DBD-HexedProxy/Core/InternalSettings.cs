namespace HexedProxy.Core
{
    internal class InternalSettings
    {
        public static string PlayerName = "NOT LOGGED IN";

        public static bool SpoofRegion = false;
        public static int TargetQueueRegion = 0;
        public static readonly string[] AvailableRegions =
        [
            "all",
        ];

        public static bool SpoofRank = false;
        public static int TargetRank = 1;

        public static bool UnlockCosmetics = false;
        public static bool UnlockItems = false;
        public static bool UnlockLevel = false;

        public static int SelectedGuiCategory = 0;
    }
}
