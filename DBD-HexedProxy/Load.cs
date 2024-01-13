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

            ProxyManager.ToggleCertificate(false);
            ProxyManager.ToggleCertificate(true);

            Task.Run(ProcessListener);

            new GUI("Hexed", true).Start().Wait();
        }

        private static async Task ProcessListener()
        {
            while (true) 
            {
                string[] ProcessList = new[]
                {
                    "DeadByDaylight-Win64-Shipping",
                    "DeadByDaylight-EGS-Shipping",
                };

                bool isRunning = false;

                foreach (string Process in ProcessList)
                {
                    if (Wrappers.Utils.GetProcessByName(Process) != null)
                    {
                        ProxyManager.Connect();
                        isRunning = true;
                        break;
                    }
                }

                if (!isRunning) ProxyManager.Disconnect();

                await Task.Delay(1);
            }
        }
    }
}
