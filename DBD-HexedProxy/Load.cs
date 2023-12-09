using HexedProxy.Core;
using HexedProxy.HexedServer;
using HexedServer;
using System.Runtime.InteropServices;

namespace HexedProxy
{
    internal class Load
    {
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        public static void Main()
        {
            Console.Title = EncryptUtils.RandomString(20);

            ServerHandler.Init();

            FreeConsole();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
