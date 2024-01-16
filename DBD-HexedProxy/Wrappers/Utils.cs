using Newtonsoft.Json.Linq;
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

        public static List<object> FindJsonFields(JToken token, string fieldName)
        {
            var matches = new List<object>();

            if (token.Type == JTokenType.Object)
            {
                foreach (var property in token.Children<JProperty>())
                {
                    if (property.Name == fieldName) matches.Add(property.Value);
                    else matches.AddRange(FindJsonFields(property.Value, fieldName));
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var arrayItem in token.Children())
                {
                    matches.AddRange(FindJsonFields(arrayItem, fieldName));
                }
            }

            return matches;
        }
    }
}
