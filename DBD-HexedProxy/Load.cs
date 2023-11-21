using HexedProxy.Core;
using HexedServer;
using System.Runtime.InteropServices;

namespace HexedProxy
{
    internal class Load
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void Main()
        {
            Console.Title = Encryption.RandomString(20);

            ShowWindow(GetConsoleWindow(), 0);

            //Task.Run(ServerHandler.Init).Wait();

            SaveEditor.Init();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
