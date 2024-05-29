using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendSTRoll : KdbNode {
        public KdbNodeTargetBendSTRoll() : base( KdbNodeType.TargetBendSTRoll ) { }

        public KdbNodeTargetBendSTRoll( BinaryReader reader ) : base( KdbNodeType.TargetBendSTRoll, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
