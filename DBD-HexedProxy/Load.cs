using HexedProxy.Core;
using HexedProxy.HexedServer;
using System.Runtime.InteropServices;

namespace HexedProxy
{
    internal class Load
    {
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public sealed class HexedEntry : Attribute { }

        [HexedEntry]
        public static void Main(string[] args)
        {
            if (args.Length != 1) return;

            ServerHandler.Init(args[0]);

            FreeConsole();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
