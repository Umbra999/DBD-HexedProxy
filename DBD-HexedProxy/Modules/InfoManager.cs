using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class InfoManager
    {
        public static string PlayerName = "NONE";
        public static string PlayerId = "NONE";

        public static string KillerId = "NONE";
        public static string KillerPlatform = "NONE";
        public static string KillerName = "NONE";
        public static string KillerPlatformId;

        public static string MatchRegion = "NONE";
        public static string MatchId = "NONE";

        public static void OnMatchInfoReceived(Match.ResponseRoot Match)
        {
            KillerId = Match.sideA.Length > 0 ? Match.sideA[0] : "NONE"; // add check for multiple killers
            MatchRegion = Match.region;
            MatchId = Match.matchId;
            Task.Run(async () =>
            {
                var PlayerProfile = await RequestSender.GetPlayerByCloudId(KillerId);
                if (PlayerProfile != null) 
                {
                    KillerName = PlayerProfile.playerName;
                    KillerId = PlayerProfile.userId;
   
                    if (PlayerProfile.providerPlayerNames?.steam != null) // provider is per platform, EG can only open EG and so on, needs to be fixed and other platforms need to be added
                    {
                        KillerPlatform = "Steam";

                        var Provider = await RequestSender.GetPlayerProvider(KillerId);
                        if (Provider != null) KillerPlatformId = Provider.providerId;
                    }
                }
            }).Wait();
        }

        public static void OnPlayerInfoReceived(PlayerName.ResponseRoot Player)
        {
            PlayerName = Player.playerName;
            PlayerId = Player.userId;
        }

        public static void OnQueueReceived()
        {
            KillerId = "NONE";
            KillerPlatform = "NONE";
            KillerName = "NONE";
            KillerPlatformId = null;

            MatchRegion = "NONE";
            MatchId = "NONE";
        }
    }
}
