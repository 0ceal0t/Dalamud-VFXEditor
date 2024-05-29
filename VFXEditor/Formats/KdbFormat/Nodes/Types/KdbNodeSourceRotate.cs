using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceRotate : KdbNode {
        public KdbNodeSourceRotate() : base( KdbNodeType.SourceRotate ) { }

        public KdbNodeSourceRotate( BinaryReader reader ) : base( KdbNodeType.SourceRotate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
