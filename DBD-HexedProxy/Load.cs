using HexedProxy.Core;
using HexedServer;

namespace HexedProxy
{
    internal class Load
    {
        public static void Main()
        {
            Console.Title = Encryption.RandomString(20);

            //Task.Run(ServerHandler.Init).Wait();

            new GUI("Hexed", true).Start().Wait();
        }
    }
}
