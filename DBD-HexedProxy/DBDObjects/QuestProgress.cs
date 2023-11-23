namespace HexedProxy.DBDObjects
{
    internal class QuestProgress
    {
        public class RequestRoot
        {
            public string krakenMatchId { get; set; }
            public string matchId { get; set; }
            public QuestEvent[] questEvents { get; set; }
            public string role { get; set; }
        }

        public class QuestEvent
        {
            public string parameters { get; set; }
            public string questEventId { get; set; }
            public int repetition { get; set; }
        }
    }
}
