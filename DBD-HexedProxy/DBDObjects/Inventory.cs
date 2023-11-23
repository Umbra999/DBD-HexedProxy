namespace HexedProxy.DBDObjects
{
    internal class Inventory
    {
        public class ResponseRoot
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
    }
}
