using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendRoll;

        public KdbNodeTargetBendRoll() : base() { }

        public KdbNodeTargetBendRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
