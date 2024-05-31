using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbTargetPosConstraint : KdbNode {
        public KdbTargetPosConstraint() : base( KdbNodeType.TargetPosContraint ) { }

        public KdbTargetPosConstraint( BinaryReader reader ) : base( KdbNodeType.TargetPosContraint, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
