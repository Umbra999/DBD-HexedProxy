using System.Reflection;

namespace HexedProxy.Wrappers
{
    internal class Utils
    {
        public static string GetFromResource(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"HexedProxy.Resources.{name}");
            if (stream == null)
            {
                Logger.LogError($"Failed to find resource {name}");
                return null;
            }

            using StreamReader reader = new(stream);
            string file = reader.ReadToEnd();
            return file;
        }

        public static int GetPipsForRank(int Rank)
        {
            switch (Rank) 
            {
                case 20:
                    return 0;

                case 19:
                    return 3;

                case 18:
                    return 6;

                case 17:
                    return 10;

                case 16:
                    return 14;

                case 15:
                    return 18;

                case 14:
                    return 22;

                case 13:
                    return 26;

                case 12:
                    return 30;

                case 11:
                    return 35;

                case 10:
                    return 40;

                case 9:
                    return 45;

                case 8:
                    return 50;

                case 7:
                    return 55;

                case 6:
                    return 60;

                case 5:
                    return 65;

                case 4:
                    return 70;

                case 3:
                    return 75;

                case 2:
                    return 80;

                case 1:
                    return 85;
            }

            return 0;
        }
    }
}
