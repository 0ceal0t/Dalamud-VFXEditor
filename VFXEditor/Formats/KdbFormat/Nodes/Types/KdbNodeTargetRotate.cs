using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetRotate : KdbNode {
        public KdbNodeTargetRotate() : base( KdbNodeType.TargetRotate ) { }

        public KdbNodeTargetRotate( BinaryReader reader ) : base( KdbNodeType.TargetRotate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
