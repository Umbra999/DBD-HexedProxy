namespace HexedProxy.DBDObjects
{
    internal class PlayerLevel
    {
        public class ResponseRoot
        {
            public int currentXp { get; set; }
            public int currentXpUpperBound { get; set; }
            public int level { get; set; }
            public int levelVersion { get; set; }
            public int prestigeLevel { get; set; }
            public int totalXp { get; set; }
        }
    }
}
