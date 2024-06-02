using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendSTRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendSTRoll;

        public KdbNodeTargetBendSTRoll() : base() { }

        public KdbNodeTargetBendSTRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
            new( ConnectionType.BendS ),
            new( ConnectionType.BendT ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
