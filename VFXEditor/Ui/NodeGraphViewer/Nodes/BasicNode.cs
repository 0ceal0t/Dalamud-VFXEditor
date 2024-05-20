using System.Collections.Generic;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class BasicNode : Node {
        public BasicNode() : base( "New Node" ) {
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey;
        }

        public override void Dispose() {

        }

        protected override List<Slot> GetSlots() => [
            new( this, "Item 1", 0 ),
            new( this, "Item 2", 1 ),
            new( this, "Item 3", 2 ),
            new( this, "Item 4", 3 )
        ];
    }
}
