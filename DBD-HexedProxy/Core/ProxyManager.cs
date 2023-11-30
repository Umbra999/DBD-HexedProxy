using Fiddler;
using HexedProxy.DBDObjects;
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

                        Queue.RequestRoot Queue = JsonConvert.DeserializeObject<Queue.RequestRoot>(e.GetRequestBodyAsString());

                        if (InternalSettings.MatchSnipe)
                        {
                            if (MatchSniper.ResetQueue)
                            {
                                Queue.checkOnly = false;
                                MatchSniper.ResetQueue = false;
                            }
                        }

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

                            RichPresence.RequestRoot Presence = JsonConvert.DeserializeObject<RichPresence.RequestRoot>(e.GetRequestBodyAsString());

                            Presence.online = false;

                            e.utilSetRequestBody(JsonConvert.SerializeObject(Presence, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                        }
                    }
                    break;

                case "/api/v1/archives/stories/update/quest-progress-v3":
                    {
                        e.utilDecodeRequest();

                        QuestProgress.RequestRoot Quest = JsonConvert.DeserializeObject<QuestProgress.RequestRoot>(e.GetRequestBodyAsString());

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

                Match.ResponseRoot MatchInfo = JsonConvert.DeserializeObject<Match.ResponseRoot>(e.GetResponseBodyAsString());

                InfoManager.OnMatchInfoReceived(MatchInfo);
            }
            else
            {
                switch (e.PathAndQuery)
                {
                    case "/api/v1/inventories":
                        {
                            if (InternalSettings.UnlockAll)
                            {
                                e.utilDecodeResponse();
                                Inventory.ResponseRoot Inventory = JsonConvert.DeserializeObject<Inventory.ResponseRoot>(e.GetResponseBodyAsString());

                                SaveEditor.EditMarket(Inventory);

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Inventory, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }
                        break;

                    case "/api/v1/dbd-character-data/get-all":
                        {
                            if (InternalSettings.UnlockAll)
                            {
                                e.utilDecodeResponse();
                                Profile.ResponseRoot Profile = JsonConvert.DeserializeObject<Profile.ResponseRoot>(e.GetResponseBodyAsString());

                                SaveEditor.EditGetAll(Profile);

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Profile, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }
                        break;

                    case "/api/v1/dbd-character-data/bloodweb":
                        {
                            e.utilDecodeResponse();
                            Bloodweb.ResponseRoot Bloodweb = JsonConvert.DeserializeObject<Bloodweb.ResponseRoot>(e.GetResponseBodyAsString());

                            BloodwebManager.OnBloodwebReceived(Bloodweb);

                            if (InternalSettings.UnlockAll)
                            {
                                SaveEditor.EditBloodweb(Bloodweb);

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Bloodweb, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                            }
                        }
                        break;

                    case "/api/v1/queue":
                        {
                            if (InternalSettings.MatchSnipe)
                            {
                                e.utilDecodeResponse();

                                Queue.ResponseRoot QueueInfo = JsonConvert.DeserializeObject<Queue.ResponseRoot>(e.GetResponseBodyAsString());

                                if (QueueInfo.status == "MATCHED")
                                {
                                    if (!MatchSniper.CheckQueueForTarget(QueueInfo))
                                    {
                                        MatchSniper.ResetQueue = true;
                                        e.utilSetResponseBody(JsonConvert.SerializeObject(new { status = "QUEUED", queueData = new { ETA = 10, position = 1, stable = true } }));
                                    }
                                }
                                else
                                {
                                    e.utilSetResponseBody(JsonConvert.SerializeObject(new { status = "QUEUED", queueData = new { ETA = 10, position = 1, stable = true } }));
                                }
                            }
                        }
                        break;

                    case "/api/v1/playername":
                        {
                            e.utilDecodeResponse();

                            PlayerName.ResponseRoot PlayerName = JsonConvert.DeserializeObject<PlayerName.ResponseRoot>(e.GetResponseBodyAsString());

                            RequestSender.OnDefaultHeadersReceived(e.RequestHeaders);
                            InfoManager.OnPlayerInfoReceived(PlayerName);
                        }
                        break;

                    case "/api/v1/extensions/playerLevels/getPlayerLevel":
                        {
                            if (InternalSettings.UnlockLevel)
                            {
                                e.utilDecodeResponse();
                                e.utilSetResponseBody(JsonConvert.SerializeObject(new PlayerLevel.ResponseRoot() { currentXp = 999, currentXpUpperBound = 999, level = 99, levelVersion = 1, prestigeLevel = 999, totalXp = 99999 }));
                            }
                        }
                        break;

                    case "/api/v1/wallet/currencies":
                        {
                            if (InternalSettings.UnlockCurrencies)
                            {
                                e.utilDecodeResponse();

                                Currencies.ResponseRoot Currencies = JsonConvert.DeserializeObject<Currencies.ResponseRoot>(e.GetResponseBodyAsString());

                                foreach (var Currency in Currencies.list)
                                {
                                    Currency.balance = 9999999;
                                }

                                e.utilSetResponseBody(JsonConvert.SerializeObject(Currencies));
                            }
                        }
                        break;

                    case "api/v1/players/ban/status":
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(new BanStatus.ResponseRoot() { isBanned = false }));
                        }
                        break;

                    case "/api/v1/archives/stories/update/active-node-v3":
                        {
                            e.utilDecodeResponse();

                            ActiveNode.ResponseRoot Node = JsonConvert.DeserializeObject<ActiveNode.ResponseRoot>(e.GetResponseBodyAsString());

                            TomeManager.OnActiveNodeReceived(Node); // Modify node to have no conditions, so its always being send so i can edit it up
                        }
                        break;
                }
            }
        }
    }
}
