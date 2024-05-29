using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorExpr : KdbNode {
        public KdbNodeEffectorExpr() : base( KdbNodeType.EffectorEZParamLink ) { }

        public KdbNodeEffectorExpr( BinaryReader reader ) : base( KdbNodeType.EffectorEZParamLink, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
