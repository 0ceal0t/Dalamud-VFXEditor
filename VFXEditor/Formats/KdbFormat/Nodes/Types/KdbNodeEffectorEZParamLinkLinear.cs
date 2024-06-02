using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLinkLinear : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorEZParamLinkLinear;

        public KdbNodeEffectorEZParamLinkLinear() : base() { }

        public KdbNodeEffectorEZParamLinkLinear( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
