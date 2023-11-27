using HexedProxy.Core;
using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class PrestigeManager
    {
        public static void LevelUntilPrestige(Bloodweb.ResponseRoot InitialBloodweb, int TargetPrestige)
        {
            Bloodweb.ResponseRoot currentBloodweb = InitialBloodweb;

            Task.Run(async () =>
            {
                List<string> BlockedNodes = new();
                List<string> SelectedNodes = new();

                while (InternalSettings.InstantPrestige && currentBloodweb != null && currentBloodweb.prestigeLevel < TargetPrestige) // add check for updated wallets
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
            });
        }
    }
}
