namespace HexedProxy.DBDObjects
{
    internal class Bloodweb
    {
        public class ResponseRoot
        {
            public BloodWebData bloodWebData { get; set; }
            public int bloodWebLevel { get; set; }
            public bool bloodWebLevelChanged { get; set; }
            public CharacterItem[] characterItems { get; set; }
            public string characterName { get; set; }
            public int legacyPrestigeLevel { get; set; }
            public int prestigeLevel { get; set; }
            public UpdatedWallet[] updatedWallets { get; set; }
        }

        public class BloodWebData
        {
            public string[] paths { get; set; }
            public RingData[] ringData { get; set; }
        }

        public class CharacterItem
        {
            public string itemId { get; set; }
            public int quantity { get; set; }
        }

        public class NodeData
        {
            public object contentId { get; set; }
            public string nodeId { get; set; }
            public string state { get; set; }
        }

        public class RingData
        {
            public NodeData[] nodeData { get; set; }
        }

        public class UpdatedWallet
        {
            public int balance { get; set; }
            public string currency { get; set; }
        }
    }
}
