namespace HexedProxy.DBDObjects
{
    internal class Currencies
    {
        public class ResponseRoot
        {
            public Currency[] list { get; set; }
        }

        public class Currency
        {
            public int balance { get; set; }
            public string currency { get; set; }
        }
    }
}
