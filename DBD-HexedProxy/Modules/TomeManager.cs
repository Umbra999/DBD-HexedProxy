using HexedProxy.DBDObjects;

namespace HexedProxy.Modules
{
    internal class TomeManager
    {
        private static ActiveNode.ResponseRoot SelectedNode;

        public static void OnActiveNodeReceived(ActiveNode.ResponseRoot Node)
        {
            SelectedNode = Node; 
        }

        public static ActiveNode.ResponseRoot GetSelectedNode()
        {
            return SelectedNode;
        }
    }
}
