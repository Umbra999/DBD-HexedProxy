using System.Reflection;
using System.Text;

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

        public static string FromBase64(string Data)
        {
            var base64EncodedBytes = Convert.FromBase64String(Data);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
