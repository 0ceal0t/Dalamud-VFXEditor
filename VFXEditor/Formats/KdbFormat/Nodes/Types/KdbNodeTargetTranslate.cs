using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetTranslate : KdbNode {
        public KdbNodeTargetTranslate() : base( KdbNodeType.TargetTranslate ) { }

        public KdbNodeTargetTranslate( BinaryReader reader ) : base( KdbNodeType.TargetTranslate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
