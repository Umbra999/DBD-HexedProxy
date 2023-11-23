using HexedProxy.Core;
using HexedServer;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HexedProxy
{
    internal class Load
    {
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public static void Main()
        {
            Console.Title = Encryption.RandomString(20);

            Task.Run(ServerHandler.Init).Wait();

            FreeConsole();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
