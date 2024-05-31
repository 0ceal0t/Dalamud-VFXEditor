using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetScale : KdbNode {
        public KdbNodeTargetScale() : base( KdbNodeType.TargetScale ) { }

        public KdbNodeTargetScale( BinaryReader reader ) : base( KdbNodeType.TargetScale, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.ScaleX ),
            new( ConnectionType.ScaleY ),
            new( ConnectionType.ScaleZ ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
