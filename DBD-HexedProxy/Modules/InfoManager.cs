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

        public static void OnMatchInfoReceived(JObject match)
        {
            isLeaving = false; // idk if needed, need to check without 

            MatchRegion = match["region"]?.Value<string>() ?? "NONE";
            MatchId = match["matchId"]?.Value<string>() ?? "NONE";

            List<CustomObjects.CustomPlayer> currentPlayers = new();

            void ProcessSide(JToken side, string role)
            {
                if (side == null) return;

                foreach (var uid in side.Values<string>())
                {
                    var customPlayer = new CustomObjects.CustomPlayer
                    {
                        userId = uid,
                        role = role
                    };

                    if (Players.Any(x => x.userId == uid))
                    {
                        currentPlayers.Add(customPlayer);
                        continue;
                    }

                    var playerProfile = RequestSender.GetPlayerByCloudId(uid).Result;
                    if (playerProfile == null) continue;

                    customPlayer.name = playerProfile.playerName;

                    //if (Platform == "steam") // fix for all platforms
                    //{
                    //    if (playerProfile.providerPlayerNames?.steam != null)
                    //    {
                    //        var provider = RequestSender.GetPlayerProvider(uid).Result;
                    //        if (provider != null) customPlayer.providerUrl = $"https://steamcommunity.com/profiles/{provider.providerId}";
                    //    }
                    //}

                    currentPlayers.Add(customPlayer);
                }
            }

            ProcessSide(match["sideA"], "Killer");
            ProcessSide(match["sideB"], "Survivor");

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
