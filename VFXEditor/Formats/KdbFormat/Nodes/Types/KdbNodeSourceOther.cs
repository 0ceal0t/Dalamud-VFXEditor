using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceOther : KdbNode {
        public KdbNodeSourceOther() : base( KdbNodeType.SourceOther ) { }

        public KdbNodeSourceOther( BinaryReader reader ) : base( KdbNodeType.SourceOther, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
