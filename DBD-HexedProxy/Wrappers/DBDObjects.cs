namespace HexedProxy.Wrappers
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
            public bool isEntitled { get; set; }
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

        public class DBDBanStatus
        {
            public bool isBanned { get; set; }
        }

        public class DBDMatch
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
        public class DBDRichPresence
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
