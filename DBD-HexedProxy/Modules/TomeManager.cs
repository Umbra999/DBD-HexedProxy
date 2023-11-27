using HexedProxy.Core;
using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class TomeManager
    {
        private static ActiveNode.ResponseRoot SelectedNode;
        private static QuestProgress.RequestRoot LastProgress;

        public static void OnActiveNodeReceived(ActiveNode.ResponseRoot Node)
        {
            SelectedNode = Node;
        }

        public static void OnNodeProgressSend(QuestProgress.RequestRoot Progress)
        {
            LastProgress = Progress;
        }

        public static ActiveNode.ResponseRoot GetSelectedNode()
        {
            return SelectedNode;
        }

        public static void FinishActiveNode() // dont work, i need to gen a match or something sadly
        {
            if (SelectedNode == null || SelectedNode.activeNodesFull == null || LastProgress == null) return;

            string selectedRole = LastProgress.role;

            List<QuestProgress.QuestEvent> questEvents = new();

            foreach (var fullNode in SelectedNode.activeNodesFull)
            {
                if (fullNode.objectives == null) continue;

                foreach (var objective in fullNode.objectives.Where(o => o.questEvent != null))
                {
                    foreach (var condition in objective.conditions)
                    {
                        if (condition.key == "role")
                        {
                            if (condition.value.Contains("survivor")) selectedRole = "survivor";
                            else if (condition.value.Contains("killer")) selectedRole = "killer";

                        }
                    }

                    foreach (var questEvent in objective.questEvent)
                    {
                        QuestProgress.QuestEvent newEvent = new()
                        {
                            parameters = questEvent.parameters,
                            questEventId = questEvent.questEventId,
                            repetition = objective.neededProgression
                        };
                        questEvents.Add(newEvent);
                    }
                }
            }

            Task.Run(() => RequestSender.FinishNodeChallenge(LastProgress.krakenMatchId, LastProgress.matchId, questEvents.ToArray(), selectedRole));
        }
    }
}
