using HexedProxy.Core;
using HexedProxy.DBDObjects;
using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class MatchSniper
    {
        public static bool ResetQueue = false;

        public static bool CheckQueueForTarget(JObject Queue)
        {
            bool returnValue = false;

            if (Queue["status"].Value<string>() == "MATCHED")
            {
                if (Queue["matchData"] != null)
                {
                    List<string> PlayerIds = [.. Queue["matchData"]["sideA"].Values<string>(), .. Queue["matchData"]["sideB"].Values<string>()];

                    foreach (string PlayerId in PlayerIds)
                    {
                        var PlayerProfile = RequestSender.GetPlayerByCloudId(PlayerId).Result;
                        if (PlayerProfile != null)
                        {
                            if (InternalSettings.OnlyStreamer)
                            {
                                string lowerName = PlayerProfile.playerName.ToLower();

                                if (lowerName.Contains("ttv") || lowerName.Contains("t.tv") || lowerName.Contains("t/tv")) returnValue = true;
                            }
                            else
                            {
                                if (PlayerProfile.playerName == InternalSettings.TargetSnipeParameter || PlayerProfile.userId == InternalSettings.TargetSnipeParameter) returnValue = true;

                                else if (PlayerProfile.providerPlayerNames?.steam != null)
                                {
                                    var Provider = RequestSender.GetPlayerProvider(PlayerId).Result;
                                    if (Provider != null && Provider.providerId == InternalSettings.TargetSnipeParameter) returnValue = true;
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }
    }
}
