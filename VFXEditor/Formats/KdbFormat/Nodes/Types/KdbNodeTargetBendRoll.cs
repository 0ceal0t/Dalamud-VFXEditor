using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendRoll : KdbNode {
        public KdbNodeTargetBendRoll() : base( KdbNodeType.TargetBendRoll ) { }

        public KdbNodeTargetBendRoll( BinaryReader reader ) : base( KdbNodeType.TargetBendRoll, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
