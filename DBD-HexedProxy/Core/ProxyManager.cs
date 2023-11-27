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
                case "/api/v1/queue": // dont work on party, add support for the party call
                    {
                        e.utilDecodeRequest();

                        InfoManager.OnQueueReceived();

                        DBDObjects.Queue.RequestRoot Queue = JsonConvert.DeserializeObject<DBDObjects.Queue.RequestRoot>(e.GetRequestBodyAsString());

                        if (InternalSettings.SpoofRank)
                        {
                            Queue.rank = InternalSettings.TargetRank;
                        }

                        if (InternalSettings.SpoofRegion)
                        {
                            Queue.region = InternalSettings.AvailableRegions[InternalSettings.TargetQueueRegion];
                        }

                        e.utilSetRequestBody(JsonConvert.SerializeObject(Queue, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                    }
                    break;

                case "/api/v1/me/richPresence":
                    {
                        if (InternalSettings.SpoofOffline)
                        {
                            e.utilDecodeRequest();

                            DBDObjects.RichPresence.RequestRoot Presence = JsonConvert.DeserializeObject<DBDObjects.RichPresence.RequestRoot>(e.GetRequestBodyAsString());

                            Presence.online = false;

                            e.utilSetRequestBody(JsonConvert.SerializeObject(Presence, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                    break;

                case "/api/v1/archives/stories/update/quest-progress-v3":
                    {
                        e.utilDecodeRequest();

                        DBDObjects.QuestProgress.RequestRoot Quest = JsonConvert.DeserializeObject<DBDObjects.QuestProgress.RequestRoot>(e.GetRequestBodyAsString());

                        TomeManager.OnNodeProgressSend(Quest);

                        if (InternalSettings.InstantTomes)
                        {
                            var selectedNode = TomeManager.GetSelectedNode();
                            if (selectedNode?.activeNodesFull != null)
                            {
                                foreach (var QuestEvent in Quest.questEvents)
                                {
                                    QuestEvent.repetition = 150000;

                                    foreach (var fullNode in selectedNode.activeNodesFull)
                                    {
                                        if (fullNode.objectives == null) continue;

                                        foreach (var objective in fullNode.objectives.Where(o => o.questEvent != null))
                                        {
                                            var cachedEvent = objective.questEvent.FirstOrDefault(e => e.questEventId == QuestEvent.questEventId);
                                            if (cachedEvent != null) QuestEvent.repetition = objective.neededProgression;
                                        }
                                    }
                                }
                            }

                            e.utilSetRequestBody(JsonConvert.SerializeObject(Quest, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                        }
                        else if (InternalSettings.BlockTomes)
                        {
                            foreach (var QuestEvent in Quest.questEvents)
                            {
                                QuestEvent.repetition = 0;
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

            if (e.PathAndQuery.StartsWith("/api/v1/match/")) // maybe reset on CLOSE state instead queue check?
            {
                e.utilDecodeResponse();

                DBDObjects.Match.ResponseRoot MatchInfo = JsonConvert.DeserializeObject<DBDObjects.Match.ResponseRoot>(e.GetResponseBodyAsString());

                InfoManager.OnMatchInfoReceived(MatchInfo);
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
                            e.utilDecodeResponse();
                            DBDObjects.Bloodweb.ResponseRoot Bloodweb = JsonConvert.DeserializeObject<DBDObjects.Bloodweb.ResponseRoot>(e.GetResponseBodyAsString());

                            BloodwebManager.OnBloodwebReceived(Bloodweb);

                            if (InternalSettings.UnlockItems)
                            {
                                Bloodweb.legacyPrestigeLevel = 3;
                                Bloodweb.prestigeLevel = 100;
                                // add character items if needed to it?

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Bloodweb, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }
                        break;

                    case "/api/v1/playername":
                        {
                            e.utilDecodeResponse();

                            DBDObjects.PlayerName.ResponseRoot PlayerName = JsonConvert.DeserializeObject<DBDObjects.PlayerName.ResponseRoot>(e.GetResponseBodyAsString());

                            RequestSender.OnDefaultHeadersReceived(e.RequestHeaders);
                            InfoManager.OnPlayerInfoReceived(PlayerName);
                            InternalSettings.cachedInventory.data.playerId = PlayerName.userId; // add saveeditor module for this
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

                            DBDObjects.ActiveNode.ResponseRoot Node = JsonConvert.DeserializeObject<DBDObjects.ActiveNode.ResponseRoot>(e.GetResponseBodyAsString());

                            TomeManager.OnActiveNodeReceived(Node);
                        }
                        break;
                }
            }
        }
    }
}
