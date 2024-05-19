using ImGuiNET;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Ui.NodeGraphViewer;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat {
    public class KdbFile : FileManagerFile {
        public readonly NodeGraphViewer NodeGraph = new();

        public KdbFile( BinaryReader reader, bool verify ) : base() {

        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {
            if( ImGui.Button( "New" ) ) {
                NodeGraph.AddNodeToActiveCanvas( new BasicNode() );
            }

            NodeGraph.Draw();
        }
    }
}
