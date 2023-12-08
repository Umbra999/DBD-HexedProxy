using HexedProxy.DBDObjects;
using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class InfoManager
    {
        public static string PlayerName = "NONE";
        public static string PlayerId = "NONE";
        public static string Platform = "NONE";

        public static List<CustomObjects.CustomPlayer> Players = new();

        public static string MatchRegion = "NONE";
        public static string MatchId = "NONE";

        private static bool isLeaving = false;

        public static void OnMatchInfoReceived(JObject Match) // unreliable list adding cuz its async
        {
            isLeaving = false;

            MatchRegion = Match["region"] == null ? "NONE" : Match["region"].Value<string>();
            MatchId = Match["matchId"] == null ? "NONE" : Match["matchId"].Value<string>();

            List<CustomObjects.CustomPlayer> currentPlayers = new();

            if (Match["sideA"] != null)
            {
                foreach (var uid in Match["sideA"].Values<string>())
                {
                    //if (Players.Any(x => x.userId == uid)) continue;

                    CustomObjects.CustomPlayer customPlayer = new()
                    {
                        userId = uid,
                        role = "Killer"
                    };

                    var PlayerProfile = RequestSender.GetPlayerByCloudId(uid).Result;
                    if (PlayerProfile == null) continue;

                    customPlayer.name = PlayerProfile.playerName;

                    switch (Platform)
                    {
                        case "steam":
                            if (PlayerProfile.providerPlayerNames?.steam != null)
                            {
                                var Provider = RequestSender.GetPlayerProvider(uid).Result;
                                if (Provider != null) customPlayer.providerUrl = $"https://steamcommunity.com/profiles/{Provider.providerId}";
                            }
                            break;

                            // Add other platfomrms here
                    }

                    currentPlayers.Add(customPlayer);
                }
            }

            if (Match["sideB"] != null)
            {
                foreach (var uid in Match["sideB"].Values<string>())
                {
                    //if (Players.Any(x => x.userId == uid)) continue;

                    CustomObjects.CustomPlayer customPlayer = new()
                    {
                        userId = uid,
                        role = "Survivor"
                    };

                    var PlayerProfile = RequestSender.GetPlayerByCloudId(uid).Result;
                    if (PlayerProfile == null) continue;

                    customPlayer.name = PlayerProfile.playerName;

                    switch (Platform)
                    {
                        case "steam":
                            if (PlayerProfile.providerPlayerNames?.steam != null)
                            {
                                var Provider = RequestSender.GetPlayerProvider(uid).Result;
                                if (Provider != null) customPlayer.providerUrl = $"https://steamcommunity.com/profiles/{Provider.providerId}";
                            }
                            break;

                            // Add other platfomrms here
                    }

                    currentPlayers.Add(customPlayer);
                }
            }

            Players.RemoveAll(player => currentPlayers.All(cp => cp.userId != player.userId));

            foreach (var player in currentPlayers)
            {
                if (!isLeaving && !Players.Any(p => p.userId == player.userId)) Players.Add(player);
            }
        }

        public static void OnPlayerInfoReceived(JObject Player, string Provider)
        {
            PlayerName = Player["playerName"].Value<string>();
            PlayerId = Player["userId"].Value<string>();
            Platform = Provider;
        }

        public static void OnPartyStateChanged()
        {
            Players.Clear();

            MatchRegion = "NONE";
            MatchId = "NONE";

            isLeaving = true;
        }
    }
}
