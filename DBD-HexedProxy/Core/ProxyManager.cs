using Fiddler;
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

            e.bBufferResponse = true;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            if (e.RequestHeaders["x-kraken-analytics-session-id"] != null) e.RequestHeaders.Remove("x-kraken-analytics-session-id");

            switch (e.PathAndQuery)
            {
                case "/api/v1/queue":
                    {
                        e.utilDecodeRequest();

                        DBDObjects.DBDQueueReq Queue = JsonConvert.DeserializeObject<DBDObjects.DBDQueueReq>(e.GetRequestBodyAsString());

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
            }
        }

        private static void BeforeResponse(Session e)
        {
            if (e == null || e.oRequest == null) return;

            e.bBufferResponse = true;

            string url = e.fullUrl;

            if (!url.Contains("bhvrdbd")) return;

            switch (e.PathAndQuery)
            {
                case "/api/v1/inventories":
                    {
                        if (InternalSettings.UnlockCosmetics)
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(SaveEditor.cachedInventory));
                        }
                    }
                    break;

                case "/api/v1/dbd-character-data/get-all":
                    {
                        if (InternalSettings.UnlockItems)
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(SaveEditor.cachedProfile)); // check if userId is needed
                        }
                    }
                    break;

                case "/api/v1/dbd-character-data/bloodweb":
                    {
                        if (InternalSettings.UnlockItems)
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(SaveEditor.cachedBloodweb)); // check if userId is needed
                        }
                    }
                    break;

                case "/api/v1/playername":
                    {
                        e.utilDecodeResponse();

                        DBDObjects.DBDPlayerName PlayerName = JsonConvert.DeserializeObject<DBDObjects.DBDPlayerName>(e.GetResponseBodyAsString());

                        SaveEditor.cachedInventory.data.playerId = PlayerName.userId;
                        InternalSettings.PlayerName = PlayerName.playerName;
                    }
                    break;

                case "/api/v1/players/ban/decayAndGetDisconnectionPenaltyPoints":
                    {
                        e.utilDecodeResponse();
                        e.utilSetResponseBody(JsonConvert.SerializeObject(new DBDObjects.DBDPenaltyPoints() { penaltyPoints = 0 }));
                    }
                    break;

                case "/api/v1/extensions/playerLevels/getPlayerLevel":
                    {
                        if (InternalSettings.UnlockLevel)
                        {
                            e.utilDecodeResponse();
                            e.utilSetResponseBody(JsonConvert.SerializeObject(new DBDObjects.DBDPlayerLevel() { currentXp = 999, currentXpUpperBound = 999, level = 999, levelVersion = 1, prestigeLevel = 999, totalXp = 99999 }));
                        }
                    }
                    break;

                case "/api/v1/ranks/reset-get-pips-v2":
                    {
                        if (InternalSettings.SpoofRank)
                        {
                            e.utilDecodeResponse();

                            DBDObjects.DBDPipReset PipReset = JsonConvert.DeserializeObject<DBDObjects.DBDPipReset>(e.GetResponseBodyAsString());

                            int Pips = Utils.GetPipsForRank(InternalSettings.TargetRank);

                            PipReset.pips.survivorPips = Pips;
                            PipReset.pips.killerPips = Pips;

                            e.utilSetResponseBody(JsonConvert.SerializeObject(PipReset));
                        }
                    }
                    break;

                case "/api/v1/queue":
                    {
                        //e.utilDecodeResponse();

                        //DBDObjects.DBDQueueResp Queue = JsonConvert.DeserializeObject<DBDObjects.DBDQueueResp>(e.GetResponseBodyAsString());
                    }
                    break;

                case "/v1/players/ban/status":
                    {
                        e.utilDecodeResponse();
                        e.utilSetResponseBody(JsonConvert.SerializeObject(new DBDObjects.DBDBanStatus() { isBanned = false }));
                    }
                    break;
            }
        }
    }
}
