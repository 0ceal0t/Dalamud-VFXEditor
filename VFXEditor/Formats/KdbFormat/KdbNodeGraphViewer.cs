using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using VfxEditor.Formats.KdbFormat.Nodes;
using VfxEditor.Ui.NodeGraphViewer;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbNodeGraphViewer : NodeGraphViewer<KdbNode> {
        public KdbNodeGraphViewer() { }

        protected override void DrawUtilsBar() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) AddToCanvas( new() );
            }

            ImGui.SameLine();
            // ===================
            base.DrawUtilsBar();
        }
    }
}
