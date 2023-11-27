using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class InfoManager
    {
        public static string PlayerName = "NONE";
        public static string PlayerId = "NONE";
        public static string KillerId = "NONE";
        public static string KillerPlatform = "NONE";
        public static string KillerPlatformId = "NONE";
        public static string MatchRegion = "NONE";
        public static string MatchId = "NONE";

        public static void OnMatchInfoReceived(Match.ResponseRoot Match)
        {
            KillerId = Match.sideA[0]; // add check for multiple killers
            MatchRegion = Match.region;
            MatchId = Match.matchId;
            Task.Run(async () =>
            {
                var Provider = await RequestSender.GetPlayerProvider(KillerId);
                if (Provider != null)
                {
                    KillerPlatform = Provider.provider;
                    KillerPlatformId = Provider.providerId;
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
            MatchRegion = "NONE";
            MatchId = "NONE";
            KillerPlatform = "NONE";
            KillerPlatformId = "NONE";
        }
    }
}
