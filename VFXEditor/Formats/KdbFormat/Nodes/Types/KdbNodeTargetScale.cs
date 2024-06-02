using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetScale : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetScale;

        public KdbNodeTargetScale() : base() { }

        public KdbNodeTargetScale( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.ScaleX ),
            new( ConnectionType.ScaleY ),
            new( ConnectionType.ScaleZ ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
