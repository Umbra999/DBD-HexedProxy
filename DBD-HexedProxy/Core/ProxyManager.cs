using ClickableTransparentOverlay.Win32;
using Fiddler;
using HexedProxy.GameDumper;
using HexedProxy.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HexedProxy.Core
{
    internal class ProxyManager
    {
        public static void ToggleCertificate(bool state)
        {
            if (state)
            {
                if (!CertMaker.rootCertExists())
                {
                    CertMaker.createRootCert();
                    CertMaker.trustRootCert();
                }
            }
            else if (CertMaker.rootCertExists()) CertMaker.removeFiddlerGeneratedCerts(true);

        }

        public static void Connect()
        {
            if (FiddlerApplication.IsStarted()) return;

            FiddlerCoreStartupSettings fiddlerCoreStartupSettings = new FiddlerCoreStartupSettingsBuilder().RegisterAsSystemProxy().DecryptSSL().Build();
            FiddlerApplication.Startup(fiddlerCoreStartupSettings);

            FiddlerApplication.BeforeRequest += BeforeRequest;
            FiddlerApplication.BeforeResponse += BeforeResponse;

            UE4Parser.Initialize();
        }

        public static void Disconnect() 
        {
            FiddlerApplication.Shutdown();
            FiddlerApplication.BeforeRequest -= BeforeRequest;
            FiddlerApplication.BeforeResponse -= BeforeResponse;
        }

        private static void BeforeRequest(Session e)
        {
            if (e == null || e.oRequest == null) return;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            e.bBufferResponse = true;

            if (e.oRequest.headers["x-kraken-analytics-session-id"] != null) e.oRequest.headers.Remove("x-kraken-analytics-session-id");

            if (e.PathAndQuery == $"/api/v1/party/player/{InfoManager.PlayerId}/state")
            {
                e.utilDecodeRequest();

                JObject Party = JObject.Parse(e.GetRequestBodyAsString());

                if (Party["body"]["_postMatchmakingState"].Value<string>() == "None") InfoManager.OnPartyStateChanged(); // bad check but works for now ig?

                if (InternalSettings.SpoofRank)
                {
                    Party["body"]["_playerRank"] = InternalSettings.TargetRank;
                }

                e.utilSetRequestBody(Party.ToString());
            }
            else
            {
                switch (e.PathAndQuery)
                {
                    case "/api/v1/queue":
                        {
                            e.utilDecodeRequest();

                            JObject Queue = JObject.Parse(e.GetRequestBodyAsString());

                            if (InternalSettings.MatchSnipe)
                            {
                                if (MatchSniper.ResetQueue)
                                {
                                    Queue["checkOnly"] = false;
                                    MatchSniper.ResetQueue = false;
                                }
                            }

                            if (InternalSettings.SpoofRank)
                            {
                                Queue["rank"] = InternalSettings.TargetRank;
                            }

                            if (InternalSettings.SpoofRegion)
                            {
                                Queue["region"] = InternalSettings.AvailableRegions[InternalSettings.TargetQueueRegion];
                            }

                            e.utilSetRequestBody(Queue.ToString());
                        }
                        break;

                    case "/api/v1/me/richPresence":
                        {
                            if (InternalSettings.SpoofOffline)
                            {
                                e.utilDecodeRequest();

                                JObject Presence = JObject.Parse(e.GetRequestBodyAsString());

                                Presence["online"] = false;

                                e.utilSetRequestBody(Presence.ToString());
                            }
                        }
                        break;

                    case "/api/v1/archives/stories/update/quest-progress-v3":
                        {
                            e.utilDecodeRequest();

                            JObject Progress = JObject.Parse(e.GetRequestBodyAsString());

                            if (InternalSettings.InstantTomes)
                            {
                                TomeManager.EditNodeProgress(Progress);

                                e.utilSetRequestBody(Progress.ToString());
                            }
                            else if (InternalSettings.BlockTomes)
                            {
                                TomeManager.ResetNodeProgress(Progress);

                                e.utilSetRequestBody(Progress.ToString());
                            }
                        }
                        break;
                }
            }
        }

        private static void BeforeResponse(Session e)
        {
            if (e == null || e.oRequest == null) return;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            e.bBufferResponse = true;

            if (url.Contains("cdn.live.bhvrdbd"))
            {
                if (e.PathAndQuery.EndsWith("/itemsKillswitch.json"))
                {
                    if (InternalSettings.DisableKillswitch)
                    {
                        e.utilDecodeResponse();
                        e.Abort();
                    }
                }
            }
            else if (e.PathAndQuery.StartsWith("/api/v1/match/")) // maybe reset on CLOSE state instead queue check? and add better patch check holy
            {
                e.utilDecodeResponse();

                JObject Match = JObject.Parse(e.GetResponseBodyAsString());

                InfoManager.OnMatchInfoReceived(Match);
            }
            else
            {
                switch (e.PathAndQuery)
                {
                    case "/api/v1/inventories":
                        {
                            e.utilDecodeResponse();
                            JObject Inventory = JObject.Parse(e.GetResponseBodyAsString());

                            SaveEditor.EditMarket(Inventory);

                            e.utilSetResponseBody(Inventory.ToString());
                        }
                        break;

                    case "/api/v1/dbd-character-data/get-all":
                        {
                            e.utilDecodeResponse();
                            JObject GetAll = JObject.Parse(e.GetResponseBodyAsString());

                            SaveEditor.EditGetAll(GetAll);

                            e.utilSetResponseBody(GetAll.ToString());
                        }
                        break;

                    case "/api/v1/dbd-character-data/bloodweb":
                        {
                            e.utilDecodeResponse();
                            JObject Bloodweb = JObject.Parse(e.GetResponseBodyAsString());

                            BloodwebManager.OnBloodwebReceived(Bloodweb);

                            SaveEditor.EditBloodweb(Bloodweb);

                            e.utilSetResponseBody(Bloodweb.ToString());
                        }
                        break;

                    case "/api/v1/queue":
                        {
                            if (InternalSettings.MatchSnipe)
                            {
                                e.utilDecodeResponse();

                                JObject Queue = JObject.Parse(e.GetResponseBodyAsString());

                                if (Queue["status"].Value<string>() == "MATCHED")
                                {
                                    if (!MatchSniper.CheckQueueForTarget(Queue))
                                    {
                                        MatchSniper.ResetQueue = true;
                                        e.utilSetResponseBody(JsonConvert.SerializeObject(new { status = "QUEUED", queueData = new { ETA = 0, position = 0, stable = true } }));
                                    }
                                }
                                else
                                {
                                    e.utilSetResponseBody(JsonConvert.SerializeObject(new { status = "QUEUED", queueData = new { ETA = 0, position = 0, stable = true } }));
                                }
                            }
                        }
                        break;

                    case "/api/v1/playername":
                        {
                            e.utilDecodeResponse();

                            JObject PlayerName = JObject.Parse(e.GetResponseBodyAsString());

                            RequestSender.OnDefaultHeadersReceived(e.RequestHeaders);
                            InfoManager.OnPlayerInfoReceived(PlayerName, e.RequestHeaders["x-kraken-client-platform"]); // maybe use x-kraken-client-provider cuz steam using the same and idk about other ones
                        }
                        break;

                    case "/api/v1/archives/stories/update/active-node-v3":
                        {
                            e.utilDecodeResponse();

                            JObject Node = JObject.Parse(e.GetResponseBodyAsString());

                            TomeManager.OnActiveNodeReceived(Node);

                            if (InternalSettings.InstantTomes)
                            {
                               TomeManager.EditSelectedNode(Node);

                                e.utilSetResponseBody(Node.ToString());
                            }
                        }
                        break;
                }
            }
        }
    }
}
