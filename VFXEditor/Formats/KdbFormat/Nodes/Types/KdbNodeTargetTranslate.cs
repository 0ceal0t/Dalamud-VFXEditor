using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetTranslate : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetTranslate;

        public KdbNodeTargetTranslate() : base() { }

        public KdbNodeTargetTranslate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) { }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
