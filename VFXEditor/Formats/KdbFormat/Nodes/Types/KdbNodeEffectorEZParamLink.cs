using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLink : KdbNode {
        public override KdbNodeType Type => KdbNodeType.EffectorEZParamLink;

        public KdbNodeEffectorEZParamLink() : base() { }

        public KdbNodeEffectorEZParamLink( BinaryReader reader ) : this() { ReaderHeader( reader ); }

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
