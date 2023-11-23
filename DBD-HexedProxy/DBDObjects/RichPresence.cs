namespace HexedProxy.DBDObjects
{
    internal class RichPresence
    {
        public class RequestRoot
        {
            public string currentProvider { get; set; }
            public GameSpecificData gameSpecificData { get; set; }
            public string gameState { get; set; }
            public string gameVersion { get; set; }
            public bool online { get; set; }
            public string userType { get; set; }
        }

        public class GameSpecificData
        {
            public string richPresencePlatform { get; set; }
            public string richPresenceStatus { get; set; }
        }
    }
}
