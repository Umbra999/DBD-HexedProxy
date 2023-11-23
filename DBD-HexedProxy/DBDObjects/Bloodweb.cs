namespace HexedProxy.DBDObjects
{
    internal class Bloodweb
    {
        public class ResponseRoot
        {
            public bool bloodWebLevelChanged { get; set; }
            public object[] updatedWallets { get; set; }
            public string characterName { get; set; }
            public int bloodWebLevel { get; set; }
            public int prestigeLevel { get; set; }
            public BloodwebData bloodWebData { get; set; }
            public Characteritem[] characterItems { get; set; }
            public int legacyPrestigeLevel { get; set; }
        }

        public class BloodwebData
        {
            public string[] paths { get; set; }
            public Ringdata[] ringData { get; set; }
        }

        public class Ringdata
        {
            public Nodedata[] nodeData { get; set; }
        }

        public class Nodedata
        {
            public object contentId { get; set; }
            public string nodeId { get; set; }
            public string state { get; set; }
        }

        public class Characteritem
        {
            public string itemId { get; set; }
            public int quantity { get; set; }
        }
    }
}
