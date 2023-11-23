namespace HexedProxy.DBDObjects
{
    internal class Match
    {
        public class ResponseRoot
        {
            public string category { get; set; }
            public int churn { get; set; }
            public long creationDateTime { get; set; }
            public string creator { get; set; }
            public CustomData customData { get; set; }
            public object geolocation { get; set; }
            public string matchId { get; set; }
            public MatchProps props { get; set; }
            public string reason { get; set; }
            public string region { get; set; }
            public int schema { get; set; }
            public string[] sideA { get; set; }
            public string[] sideB { get; set; }
            public string status { get; set; }
            public int version { get; set; }
        }

        public class CustomData
        {
            public string SessionSettings { get; set; }
        }

        public class MatchProps
        {
            public string CrossplayOptOut { get; set; }
            public string EncryptionKey { get; set; }
            public int countA { get; set; }
            public int countB { get; set; }
            public string gameMode { get; set; }
            public bool isDedicated { get; set; }
            public string platform { get; set; }
        }
    }
}
