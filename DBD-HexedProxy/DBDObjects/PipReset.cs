namespace HexedProxy.DBDObjects
{
    internal class PipReset
    {
        public class ResponseRoot
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
    }
}
