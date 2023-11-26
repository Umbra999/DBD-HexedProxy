using Fiddler;
using HexedProxy.Modules;
using HexedProxy.Wrappers;
using Newtonsoft.Json;

namespace HexedProxy.Core
{
    internal class ProxyManager
    {
        public static void Connect()
        {
            if (FiddlerApplication.IsStarted()) return;

            if (!CertMaker.rootCertExists())
            {
                CertMaker.createRootCert();
                CertMaker.trustRootCert();
            }

            FiddlerCoreStartupSettings fiddlerCoreStartupSettings = new FiddlerCoreStartupSettingsBuilder().RegisterAsSystemProxy().DecryptSSL().Build();
            FiddlerApplication.Startup(fiddlerCoreStartupSettings);

            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += BeforeResponse;
        }

        public static void Disconnect() 
        {
            if (!FiddlerApplication.IsStarted()) return;

            FiddlerApplication.Shutdown();
            FiddlerApplication.BeforeRequest -= BeforeRequest;
            FiddlerApplication.BeforeResponse -= BeforeResponse;

            if (CertMaker.rootCertExists()) CertMaker.removeFiddlerGeneratedCerts(true);
        }

        private static void BeforeRequest(Session e)
        {
            if (e == null || e.oRequest == null) return;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            e.bBufferResponse = true;

            if (e.oRequest.headers["x-kraken-analytics-session-id"] != null) e.oRequest.headers.Remove("x-kraken-analytics-session-id");

            switch (e.PathAndQuery)
            {
                case "/api/v1/queue":
                    {
                        e.utilDecodeRequest();

                        DBDObjects.Queue.RequestRoot Queue = JsonConvert.DeserializeObject<DBDObjects.Queue.RequestRoot>(e.GetRequestBodyAsString());

                        if (InternalSettings.SpoofRank)
                        {
                            Queue.rank = InternalSettings.TargetRank;
                        }

                        if (InternalSettings.SpoofRegion)
                        {
                            Queue.region = InternalSettings.AvailableRegions[InternalSettings.TargetQueueRegion];
                        }

                        e.utilSetRequestBody(JsonConvert.SerializeObject(Queue));
                    }
                    break;

                case "/api/v1/me/richPresence":
                    {
                        if (InternalSettings.SpoofOffline)
                        {
                            e.utilDecodeRequest();

                            DBDObjects.RichPresence.RequestRoot Presence = JsonConvert.DeserializeObject<DBDObjects.RichPresence.RequestRoot>(e.GetRequestBodyAsString());

                            Presence.online = false;

                            e.utilSetRequestBody(JsonConvert.SerializeObject(Presence));
                        }
                    }
                    break;

                case "/api/v1/archives/stories/update/quest-progress-v3":
                    {
                        if (InternalSettings.InstantTomes)
                        {
                            e.utilDecodeRequest();

                            DBDObjects.QuestProgress.RequestRoot Quest = JsonConvert.DeserializeObject<DBDObjects.QuestProgress.RequestRoot>(e.GetRequestBodyAsString());

                            foreach (var QuestEvent in Quest.questEvents)
                            {
                                QuestEvent.repetition = 999999;

                                if (InternalSettings.ActiveTomeData?.activeNodesFull != null)
                                {
                                    foreach (var fullNode in InternalSettings.ActiveTomeData.activeNodesFull)
                                    {
                                        if (fullNode?.objectives == null) continue;

                                        foreach (var objective in fullNode.objectives.Where(o => o?.questEvent != null))
                                        {
                                            var cachedEvent = objective.questEvent.FirstOrDefault(e => e.questEventId == QuestEvent.questEventId);

                                            if (cachedEvent != null)
                                            {
                                                QuestEvent.repetition = objective.neededProgression;
                                                Wrappers.Logger.LogDebug($"{QuestEvent.questEventId} set to: {objective.neededProgression}");
                                            }
                                        }
                                    }
                                }
                            }

                            e.utilSetRequestBody(JsonConvert.SerializeObject(Quest, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                    break;
            }
        }

        private static void BeforeResponse(Session e)
        {
            if (e == null || e.oRequest == null) return;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            e.bBufferResponse = true;

            if (e.PathAndQuery.StartsWith("/api/v1/match/"))
            {
                e.utilDecodeResponse();

                DBDObjects.Match.ResponseRoot MatchInfo = JsonConvert.DeserializeObject<DBDObjects.Match.ResponseRoot>(e.GetResponseBodyAsString());

                InternalSettings.KillerId = MatchInfo.sideA[0]; // add check for multiple killers
                InternalSettings.MatchRegion = MatchInfo.region;
                InternalSettings.MatchId = MatchInfo.matchId;
                Task.Run(async () =>
                {
                    var Provider = await RequestSender.GetProvider(InternalSettings.KillerId);
                    if (Provider != null)
                    {
                        InternalSettings.KillerPlatform = Provider.provider;
                        InternalSettings.KillerPlatformId = Provider.providerId;
                    }
                }).Wait();
            }
            else
            {
                switch (e.PathAndQuery)
                {
                    case "/api/v1/inventories":
                        {
                            if (InternalSettings.UnlockCosmetics)
                            {
                                e.utilDecodeResponse();
                                e.utilSetResponseBody(JsonConvert.SerializeObject(InternalSettings.cachedInventory));
                            }
                        }
                        break;

                    case "/api/v1/dbd-character-data/get-all":
                        {
                            if (InternalSettings.UnlockItems)
                            {
                                e.utilDecodeResponse();
                                e.utilSetResponseBody(JsonConvert.SerializeObject(InternalSettings.cachedProfile));
                            }
                        }
                        break;

                    case "/api/v1/dbd-character-data/bloodweb":
                        {
                            if (InternalSettings.UnlockItems)
                            {
                                e.utilDecodeResponse();
                                //e.utilSetResponseBody(JsonConvert.SerializeObject(InternalSettings.cachedBloodweb));

                                DBDObjects.Bloodweb.ResponseRoot Bloodweb = JsonConvert.DeserializeObject<DBDObjects.Bloodweb.ResponseRoot>(e.GetResponseBodyAsString());

                                Bloodweb.legacyPrestigeLevel = 3;
                                Bloodweb.prestigeLevel = 100;
                                Bloodweb.bloodWebLevel = 50;

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Bloodweb));
                            }

                            if (InternalSettings.InstantPrestige)
                            {
                                e.utilDecodeResponse();

                                DBDObjects.Bloodweb.ResponseRoot Bloodweb = JsonConvert.DeserializeObject<DBDObjects.Bloodweb.ResponseRoot>(e.GetResponseBodyAsString());

                                PrestigeManager.LevelUntilPrestige(Bloodweb, 100);
                            }
                        }
                        break;

                    case "/api/v1/playername":
                        {
                            e.utilDecodeResponse();

                            DBDObjects.PlayerName.ResponseRoot PlayerName = JsonConvert.DeserializeObject<DBDObjects.PlayerName.ResponseRoot>(e.GetResponseBodyAsString());

                            InternalSettings.cachedInventory.data.playerId = PlayerName.userId;
                            InternalSettings.PlayerName = PlayerName.playerName;
                            InternalSettings.PlayerId = PlayerName.userId;
                            RequestSender.headers = e.RequestHeaders;
                        }
                        break;

                    case "/api/v1/extensions/playerLevels/getPlayerLevel":
                        {
                            if (InternalSettings.UnlockLevel)
                            {
                                e.utilDecodeResponse();
                                e.utilSetResponseBody(JsonConvert.SerializeObject(new DBDObjects.PlayerLevel.ResponseRoot() { currentXp = 999, currentXpUpperBound = 999, level = 99, levelVersion = 1, prestigeLevel = 999, totalXp = 99999 }));
                            }
                        }
                        break;

                    case "/api/v1/wallet/currencies":
                        {
                            if (InternalSettings.UnlockCurrencies)
                            {
                                e.utilDecodeResponse();

                                DBDObjects.Currencies.ResponseRoot Currencies = JsonConvert.DeserializeObject<DBDObjects.Currencies.ResponseRoot>(e.GetResponseBodyAsString());

                                foreach (var Currency in Currencies.list)
                                {
                                    Currency.balance = 9999999;
                                }

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Currencies));
                            }
                        }
                        break;

                    case "/api/v1/ranks/reset-get-pips-v2": // maybe remove and make queue only spoof
                        {
                            if (InternalSettings.SpoofRank)
                            {
                                e.utilDecodeResponse();

                                DBDObjects.PipReset.ResponseRoot PipReset = JsonConvert.DeserializeObject<DBDObjects.PipReset.ResponseRoot>(e.GetResponseBodyAsString());

                                int Pips = Utils.GetPipsForRank(InternalSettings.TargetRank);

                                PipReset.pips.survivorPips = Pips;
                                PipReset.pips.killerPips = Pips;

                                e.utilSetResponseBody(JsonConvert.SerializeObject(PipReset, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }
                        break;

                    case "api/v1/players/ban/status":
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(new DBDObjects.BanStatus.ResponseRoot() { isBanned = false }));
                        }
                        break;

                    case "/api/v1/archives/stories/update/active-node-v3":
                        {
                            e.utilDecodeResponse();
                            try
                            {
                                InternalSettings.ActiveTomeData = JsonConvert.DeserializeObject<DBDObjects.ActiveNode.ResponseRoot>(e.GetResponseBodyAsString());
                            }
                            catch (Exception ex) { Wrappers.Logger.LogError(ex); }
                        }
                        break;

                    case "/api/v1/queue":
                        {
                            InternalSettings.KillerId = "NONE";
                            InternalSettings.MatchRegion = "NONE";
                            InternalSettings.MatchId = "NONE";
                            InternalSettings.KillerPlatform = "NONE";
                            InternalSettings.KillerPlatformId = "NONE";
                        }
                        break;
                }
            }
        }
    }
}
