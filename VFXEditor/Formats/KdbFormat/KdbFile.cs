using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.IO;
using System.Linq;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbFile : FileManagerFile {
        public readonly KdbNodeGraphViewer NodeGraph = new();

        public KdbFile( BinaryReader reader, bool verify ) : base() {

        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {
            ImGui.Separator();
            using( var graphChild = ImRaii.Child( "GraphChild", new( -1, ImGui.GetContentRegionAvail().Y / 2f ) ) ) {
                NodeGraph.Draw();
            }

            using var selectedChild = ImRaii.Child( "SelectedChild" );
            if( NodeGraph.Canvas.SelectedNodes.Count == 0 ) return;

            var node = NodeGraph.Canvas.SelectedNodes.FirstOrDefault();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) NodeGraph.Canvas.RemoveNode( node );
            }
            node.Draw();
        }
    }
}
