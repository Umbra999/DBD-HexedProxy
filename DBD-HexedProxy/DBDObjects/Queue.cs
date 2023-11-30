namespace HexedProxy.DBDObjects
{
    internal class Queue
    {
        public class RequestRoot
        {
            public object[] additionalUserIds { get; set; }
            public string category { get; set; }
            public bool checkOnly { get; set; }
            public int countA { get; set; }
            public int countB { get; set; }
            public Latency[] latencies { get; set; }
            public string platform { get; set; }
            public QueueProps props { get; set; }
            public int rank { get; set; }
            public string region { get; set; }
            public string side { get; set; }
        }

        public class Latency
        {
            public int latency { get; set; }
            public string regionName { get; set; }
        }

        public class QueueProps
        {
            public string CrossplayOptOut { get; set; }
            public string characterName { get; set; }
        }



        public class ResponseRoot
        {
            public MatchData matchData { get; set; }
            public QueueData queueData { get; set; }
            public string status { get; set; }
        }
        public class MatchData
        {
            public string category { get; set; }
            public int churn { get; set; }
            public long creationDateTime { get; set; }
            public string creator { get; set; }
            public object customData { get; set; }
            public object geolocation { get; set; }
            public string matchId { get; set; }
            public string[] playerHistory { get; set; }
            public Props props { get; set; }
            public int rank { get; set; }
            public string reason { get; set; }
            public string region { get; set; }
            public int schema { get; set; }
            public string[] sideA { get; set; }
            public string[] sideB { get; set; }
            public Skill skill { get; set; }
            public string status { get; set; }
            public int version { get; set; }
        }

        public class Props
        {
            public string CrossplayOptOut { get; set; }
            public string EncryptionKey { get; set; }
            public string characterName { get; set; }
            public int countA { get; set; }
            public int countB { get; set; }
            public string gameMode { get; set; }
            public bool isDedicated { get; set; }
            public string platform { get; set; }
        }

        public class QueueData
        {
            //public int ETA { get; set; }
            public int position { get; set; }
            public bool stable { get; set; }
        }

        public class Rating
        {
            public double RD { get; set; }
            public int rating { get; set; }
            public double volatility { get; set; }
        }

        public class Skill
        {
            public string[] countries { get; set; }
            public bool isProtected { get; set; }
            public bool isSuspicious { get; set; }
            //public double latitude { get; set; }
            //public double longitude { get; set; }
            public int rank { get; set; }
            //public Rating rating { get; set; }
            public int version { get; set; }
            public int x { get; set; }
        }
    }
}
