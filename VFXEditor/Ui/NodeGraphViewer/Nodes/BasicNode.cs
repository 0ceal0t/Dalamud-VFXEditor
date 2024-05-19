using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class BasicNode : Node {
        public new const string nodeType = "BasicNode";
        public override string Type { get; } = BasicNode.nodeType;

        protected string _newDescription = null;

        public BasicNode() : base() {
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey;
        }
        protected override NodeInteractionFlags DrawBody( Vector2 pNodeOSP, float pCanvasScaling ) {
            ImGui.PushStyleColor( ImGuiCol.Text, NodeUtils.Colors.NodeText );

            ImGui.PushTextWrapPos();
            ImGui.TextUnformatted( Content.GetDescription() );
            ImGui.PopTextWrapPos();

            ImGui.PopStyleColor();
            return NodeInteractionFlags.None;
        }

        public override void Dispose() {

        }

        public override void OnDelete() {

        }
    }
}
