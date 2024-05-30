using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorExpr : KdbNode {
        public KdbNodeEffectorExpr() : base( KdbNodeType.EffectorExpr ) { }

        public KdbNodeEffectorExpr( BinaryReader reader ) : base( KdbNodeType.EffectorExpr, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input, true ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
