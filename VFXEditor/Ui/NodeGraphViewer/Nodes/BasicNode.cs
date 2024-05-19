using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class BasicNode : Node {
        public BasicNode() : base( "New Node" ) {
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey;
        }
        protected override NodeInteractionFlags DrawBody( Vector2 pNodeOSP, float pCanvasScaling ) {
            return NodeInteractionFlags.None;
        }

        public override void Dispose() {

        }

        public override void OnDelete() {

        }
    }
}
