using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetOrientationConstraint : KdbNode {
        public KdbNodeTargetOrientationConstraint() : base( KdbNodeType.TargetOrientationConstraint ) { }

        public KdbNodeTargetOrientationConstraint( BinaryReader reader ) : base( KdbNodeType.TargetOrientationConstraint, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
        ];
    }
}
