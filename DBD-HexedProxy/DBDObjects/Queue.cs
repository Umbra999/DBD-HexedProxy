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
    }
}
