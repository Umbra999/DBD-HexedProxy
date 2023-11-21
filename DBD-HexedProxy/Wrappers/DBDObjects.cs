namespace HexedUnlocker.Wrappers
{
    internal class DBDObjects
    {
        public class DBDBloodweb
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


        public class DBDInventory
        {
            public string code { get; set; }
            public InventoryData data { get; set; }
            public string message { get; set; }
        }

        public class InventoryData
        {
            public InventoryItems[] inventory { get; set; }
            public string playerId { get; set; }
        }

        public class InventoryItems
        {
            public object lastUpdatedAt { get; set; }
            public string objectId { get; set; }
            public int quantity { get; set; }
        }


        public class DBDProfile
        {
            public Character[] list { get; set; }
        }

        public class Character
        {
            public string characterName { get; set; }
            public int legacyPrestigeLevel { get; set; }
            public Characteritem[] characterItems { get; set; }
            public int bloodWebLevel { get; set; }
            public Bloodwebdata bloodWebData { get; set; }
            public int prestigeLevel { get; set; }
        }

        public class Bloodwebdata
        {
            public string[] paths { get; set; }
            public Ringdata[] ringData { get; set; }
        }

        public class DBDPlayerName
        {
            public string playerName { get; set; }
            public string userId { get; set; }
        }

        public class DBDPenaltyPoints
        {
            public int penaltyPoints { get; set; }
        }

        public class DBDPlayerLevel
        {
            public int currentXp { get; set; }
            public int currentXpUpperBound { get; set; }
            public int level { get; set; }
            public int levelVersion { get; set; }
            public int prestigeLevel { get; set; }
            public int totalXp { get; set; }
        }

        public class DBDPipReset
        {
            public long nextRankResetDate { get; set; }
            public Pip pips { get; set; }
            public bool seasonRefresh { get; set; }
        }

        public class Pip
        {
            public int killerPips { get; set; }
            public int survivorPips { get; set; }
        }


        public class DBDQueue
        {
            public object[] additionalUserIds { get; set; }
            public string category { get; set; }
            public bool checkOnly { get; set; }
            public int countA { get; set; }
            public int countB { get; set; }
            public Latency[] latencies { get; set; }
            public string platform { get; set; }
            public Props props { get; set; }
            public int rank { get; set; }
            public string region { get; set; }
            public string side { get; set; }
        }

        public class Latency
        {
            public int latency { get; set; }
            public string regionName { get; set; }
        }

        public class Props
        {
            public string CrossplayOptOut { get; set; }
            public string characterName { get; set; }
        }

        public class DBDBanStatus
        {
            public bool isBanned { get; set; }
        }
    }
}
