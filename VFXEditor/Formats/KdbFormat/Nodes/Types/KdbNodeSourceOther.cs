using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceOther : KdbNode {
        public KdbNodeSourceOther() : base( KdbNodeType.SourceOther ) { }

        public KdbNodeSourceOther( BinaryReader reader ) : base( KdbNodeType.SourceOther, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
