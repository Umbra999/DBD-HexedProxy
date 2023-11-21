using HexedProxy.Core;
using HexedServer;
using System.Runtime.InteropServices;

namespace HexedProxy
{
    internal class Load
    {
        public static void Main()
        {
            Console.Title = Encryption.RandomString(20);

            //Task.Run(ServerHandler.Init).Wait();

            SaveEditor.Init();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
