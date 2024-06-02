using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetRotate : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetRotate;

        public KdbNodeTargetRotate() : base() { }

        public KdbNodeTargetRotate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.RotateQuat ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
