using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class BloodwebManager
    {
        private static Bloodweb.ResponseRoot SelectedBloodweb;
        public static int TargetPrestige = 0;

        public static void OnBloodwebReceived(Bloodweb.ResponseRoot Bloodweb)
        {
            SelectedBloodweb = Bloodweb;
        }

        public static int GetAvailablePrestiges()
        {
            if (SelectedBloodweb == null) return 0;

            return 100 - SelectedBloodweb.prestigeLevel;
        }

        public static int GetCurrentPrestige()
        {
            if (SelectedBloodweb == null) return 0;

            return SelectedBloodweb.prestigeLevel;
        }

        public static void AddPrestigeLevels()
        {
            if (SelectedBloodweb == null) return;

            Task.Run(() => LevelUntilPrestige(SelectedBloodweb, TargetPrestige));
        }

        public static string GetSelectedCharacter()
        {
            if (SelectedBloodweb == null) return "NONE";

            return SelectedBloodweb.characterName;
        }

        private static async Task LevelUntilPrestige(Bloodweb.ResponseRoot InitialBloodweb, int TargetPrestige)
        {
            Bloodweb.ResponseRoot currentBloodweb = InitialBloodweb;

            List<string> BlockedNodes = new();
            List<string> SelectedNodes = new();

            while (currentBloodweb != null && (currentBloodweb.prestigeLevel < TargetPrestige || currentBloodweb.bloodWebLevel < 50)) // add check for updated wallets
            {
                BlockedNodes.Clear();
                SelectedNodes.Clear();

                foreach (var ringdata in currentBloodweb.bloodWebData.ringData)
                {
                    foreach (var nodedata in ringdata.nodeData)
                    {
                        if (nodedata.nodeId == "0" && nodedata.state == "Available") SelectedNodes.Add(nodedata.nodeId);
                        else BlockedNodes.Add(nodedata.nodeId);
                    }
                }

                currentBloodweb = await RequestSender.FinishBloodweb(currentBloodweb.characterName, BlockedNodes.ToArray(), SelectedNodes.ToArray());
            }
        }
    }
}
