using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class TomeManager
    {
        private static JObject SelectedNode;

        public static void OnActiveNodeReceived(JObject Node)
        {
            SelectedNode = Node; 
        }

        public static void EditNodeProgress(JObject Progress)
        {
            if (SelectedNode?["activeNodesFull"] != null)
            {
                foreach (var QuestEvent in Progress["questEvents"])
                {
                    foreach (var fullNode in SelectedNode["activeNodesFull"])
                    {
                        if (fullNode["objectives"] == null) continue;

                        foreach (var objective in fullNode["objectives"].Where(o => o["questEvent"] != null))
                        {
                            var cachedEvent = objective["questEvent"].FirstOrDefault(e => e["questEventId"].Value<string>() == QuestEvent["questEventId"].Value<string>());
                            if (cachedEvent != null) QuestEvent["repetition"] = objective["neededProgression"];
                        }
                    }
                }
            }
        }

        public static void ResetNodeProgress(JObject Progress)
        {
            foreach (var QuestEvent in Progress["questEvents"])
            {
                QuestEvent["repetition"] = 0;
            }
        }

        public static void EditSelectedNode(JObject Node)
        {
            foreach (var activeNode in Node["activeNodesFull"])
            {
                if (activeNode["objectives"] == null) continue;

                foreach (var objective in activeNode["objectives"])
                {
                    objective["conditions"] = new JArray();
                }
            }
        }
    }
}
