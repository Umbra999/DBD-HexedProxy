using Fiddler;
using HexedProxy.Core;
using HexedProxy.GameDumper;
using HexedProxy.HexedServer;
using HexedProxy.Wrappers;
using System.Diagnostics;
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
                try // EAC is gay idfk why it breaks without
                {
                    Process EACBoot = Utils.GetProcessByName("DeadByDaylight");
                    if (EACBoot != null && EACBoot.MainModule != null && EACBoot.MainModule.FileName != null) UE4Parser.LastKnownDirectoryPath = EACBoot.MainModule.FileName.Replace("DeadByDaylight.exe", "");
                }
                catch { }

                string[] ProcessList = new[]
                {
                    "DeadByDaylight-Win64-Shipping",
                    "DeadByDaylight-EGS-Shipping",
                    "DeadByDaylight-WinGDK-Shipping"
                };

                bool isRunning = false;

                foreach (string Process in ProcessList)
                {
                    if (Utils.GetProcessByName(Process) != null)
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
