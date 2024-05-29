using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using VfxEditor.Ui.NodeGraphViewer;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbNodeGraphViewer : NodeGraphViewer<KdbNode> {
        public KdbNodeGraphViewer() { }

        protected override void DrawUtilsBar() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {

                }
            }

            ImGui.SameLine();
            // ===================
            base.DrawUtilsBar();
        }
    }
}
