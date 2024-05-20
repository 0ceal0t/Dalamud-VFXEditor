namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class BasicNode : Node {
        public BasicNode() : base( "New Node", 4 ) {
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey;
        }

        public override void Dispose() {

        }
    }
}
