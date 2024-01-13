using System.Diagnostics;
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

        public static Process GetProcessByName(string Name)
        {
            Process[] AllProcesses = Process.GetProcessesByName(Name);
            if (AllProcesses != null && AllProcesses.Length > 0) return AllProcesses[0];

            return null;
        }
    }
}
