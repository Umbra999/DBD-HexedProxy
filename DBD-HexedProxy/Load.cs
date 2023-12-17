using HexedProxy.Core;
using HexedServer;
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
            if (!File.Exists("Key.Hexed")) return;

            ServerHandler.Init();

            FreeConsole();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
