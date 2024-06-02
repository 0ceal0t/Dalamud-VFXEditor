using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetOrientationConstraint : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetOrientationConstraint;

        public KdbNodeTargetOrientationConstraint() : base() { }

        public KdbNodeTargetOrientationConstraint( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
        ];
    }
}
