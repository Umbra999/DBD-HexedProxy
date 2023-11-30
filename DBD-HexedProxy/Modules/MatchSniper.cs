using HexedProxy.Core;
using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class MatchSniper
    {
        public static bool ResetQueue = false;

        public static bool CheckQueueForTarget(Queue.ResponseRoot Queue)
        {
            bool returnValue = false;

            Task.Run(async () =>
            {
                if (Queue.status == "MATCHED")
                {
                    if (Queue.matchData != null)
                    {
                        List<string> PlayerIds = [.. Queue.matchData.sideA, .. Queue.matchData.sideB];

                        foreach (string PlayerId in PlayerIds)
                        {
                            var PlayerProfile = await RequestSender.GetPlayerByCloudId(PlayerId);
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
                                        var Provider = await RequestSender.GetPlayerProvider(PlayerId);
                                        if (Provider != null && Provider.providerId == InternalSettings.TargetSnipeParameter) returnValue = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }).Wait();

            return returnValue;
        }
    }
}
