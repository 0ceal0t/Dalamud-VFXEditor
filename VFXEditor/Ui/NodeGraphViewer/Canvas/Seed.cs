using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Content;

namespace VfxEditor.Ui.NodeGraphViewer.Canvas {
    public class Seed {
        public string nodeType;
        public NodeContent nodeContent;
        public bool isEdgeConnected;
        public Vector2? ofsToPrevNode;

        private Seed() { }
        public Seed( string nodeType, NodeContent nodeContent, bool isEdgeConnected = true, Vector2? ofsToPrevNode = null ) {
            this.nodeType = nodeType;
            this.nodeContent = nodeContent;
            this.isEdgeConnected = isEdgeConnected;
            this.ofsToPrevNode = ofsToPrevNode;
        }
    }
}
