using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorExpr : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorExpr;

        public KdbNodeEffectorExpr() : base() { }

        public KdbNodeEffectorExpr( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {

        }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input, true ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
