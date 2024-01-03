using Newtonsoft.Json.Linq;

namespace HexedProxy.Modules
{
    internal class BloodwebManager
    {
        private static JObject SelectedBloodweb;
        public static int TargetPrestige = 0;

        public static void OnBloodwebReceived(JObject Bloodweb)
        {
            SelectedBloodweb = Bloodweb;
        }

        public static int GetAvailablePrestiges()
        {
            if (SelectedBloodweb == null) return 0;

            return 100 - SelectedBloodweb["prestigeLevel"].Value<int>();
        }

        public static int GetCurrentPrestige()
        {
            if (SelectedBloodweb == null) return 0;

            return SelectedBloodweb["prestigeLevel"].Value<int>();
        }

        public static void AddPrestigeLevels()
        {
            if (SelectedBloodweb == null) return;

            Task.Run(() => LevelUntilPrestige(SelectedBloodweb, TargetPrestige));
        }

        public static string GetSelectedCharacter()
        {
            if (SelectedBloodweb == null) return "NONE";

            return SelectedBloodweb["characterName"].Value<string>();
        }

        private static async Task LevelUntilPrestige(JObject InitialBloodweb, int TargetPrestige)
        {
            JObject currentBloodweb = InitialBloodweb;

            List<string> BlockedNodes = new();
            List<string> SelectedNodes = new();

            while (currentBloodweb != null && (currentBloodweb["prestigeLevel"].Value<int>() < TargetPrestige || currentBloodweb["bloodWebLevel"].Value<int>() < 50)) // add check for updated wallets if i can afford the next prestige
            {
                BlockedNodes.Clear();
                SelectedNodes.Clear();

                foreach (var ringdata in currentBloodweb["bloodWebData"]["ringData"])
                {
                    foreach (var nodedata in ringdata["nodeData"])
                    {
                        if (nodedata["nodeId"].Value<string>() == "0" && nodedata["state"].Value<string>() == "Available") SelectedNodes.Add(nodedata["nodeId"].Value<string>());
                        else BlockedNodes.Add(nodedata["nodeId"].Value<string>());
                    }
                }

                currentBloodweb = await RequestSender.FinishBloodweb(currentBloodweb["characterName"].Value<string>(), BlockedNodes.ToArray(), SelectedNodes.ToArray());
            }
        }
    }
}
