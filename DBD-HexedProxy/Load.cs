using HexedProxy.Core;
using HexedProxy.Wrappers;
using HexedServer;

namespace HexedProxy
{
    internal class Load
    {
        public static void Main()
        {
            Console.Title = Encryption.RandomString(20);

            //Task.Run(ServerHandler.Init).Wait();

            SaveEditor.Init();

            ProxyManager.Connect();

            Logger.LogWarning("Press Enter to exit");
            Console.ReadLine();

            ProxyManager.Disconnect();
        }
    }
}
