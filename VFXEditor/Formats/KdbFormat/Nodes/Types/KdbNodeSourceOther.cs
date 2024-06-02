using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceOther : KdbNode {
        public override KdbNodeType Type => KdbNodeType.SourceOther;

        public KdbNodeSourceOther() : base() { }

        public KdbNodeSourceOther( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
