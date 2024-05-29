using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceTranslate : KdbNode {
        public KdbNodeSourceTranslate() : base( KdbNodeType.SourceTranslate ) { }

        public KdbNodeSourceTranslate( BinaryReader reader ) : base( KdbNodeType.SourceTranslate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
