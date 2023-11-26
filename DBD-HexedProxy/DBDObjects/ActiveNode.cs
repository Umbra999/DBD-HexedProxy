namespace HexedProxy.DBDObjects
{
    internal class ActiveNode
    {
        public class ResponseRoot
        {
            public Node[] activeNode { get; set; }
            public FullNode[] activeNodesFull { get; set; }
            //public ListOfNode listOfNodes { get; set; }
            public Node survivorActiveNode { get; set; }
            public Node killerActiveNode { get; set; }
        }

        public class Node
        {
            public int level { get; set; }
            public string nodeId { get; set; }
            public string storyId { get; set; }
        }

        public class FullNode
        {
            public string clientInfoId { get; set; }
            //public Coordinates coordinates { get; set; }
            public string[] neighbors { get; set; }
            //public NodeTreeCoordinate nodeTreeCoordinate { get; set; }
            public string nodeType { get; set; }
            public Objective[] objectives { get; set; }
            public Reward[] rewards { get; set; }
            public string status { get; set; }
        }

        public class Coordinates
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        public class ListOfNode
        {
            public NodeTreeCoordinate nodeTreeCoordinate { get; set; }
            public Objective[] objectives { get; set; }
            public string status { get; set; }
        }

        public class NodeTreeCoordinate
        {
            public int level { get; set; }
            public string nodeId { get; set; }
            public string storyId { get; set; }
        }

        public class Objective
        {
            public object[] conditions { get; set; }
            public int currentProgress { get; set; }
            public bool incrementWithEventRepetitions { get; set; }
            public bool isCommunityObjective { get; set; }
            public int neededProgression { get; set; }
            public string objectiveId { get; set; }
            public QuestEvent[] questEvent { get; set; }
        }

        public class QuestEvent
        {
            public string operation { get; set; }
            public string questEventId { get; set; }
            public int repetition { get; set; }
        }

        public class Reward
        {
            public int amount { get; set; }
            public string id { get; set; }
            public string type { get; set; }
        }  
    }
}
