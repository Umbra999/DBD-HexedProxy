namespace HexedProxy.DBDObjects
{
    internal class PlayerNameById
    {
        public class ResponseRoot
        {
            public string playerName { get; set; }
            public Provider providerPlayerNames { get; set; }
            public string userId { get; set; }
        }

        public class Provider
        {
            public string steam { get; set; }
            public string psn { get; set; }
            public string egs { get; set; }
            public string xbox { get; set; }
            public string nintendo { get; set; }
        }
    }
}
