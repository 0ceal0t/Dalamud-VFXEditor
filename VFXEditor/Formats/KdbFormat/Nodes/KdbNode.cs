using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Ui.NodeGraphViewer;
using VfxEditor.Ui.NodeGraphViewer.Nodes;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbNode : Node {
        public KdbNode() : base( "New Node" ) {
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey;
        }

        protected override List<Slot> GetSlots() => [
            new( this, "Item 1", 0 ),
            new( this, "Item 2", 1 ),
            new( this, "Item 3", 2 ),
            new( this, "Item 4", 3 )
        ];

        public void Draw() {
            if( ImGui.InputText( "Name", ref Name, 255 ) ) {
                SetHeader( Name );
                Style.SetSize( BodySize );
            }
        }
    }
}
