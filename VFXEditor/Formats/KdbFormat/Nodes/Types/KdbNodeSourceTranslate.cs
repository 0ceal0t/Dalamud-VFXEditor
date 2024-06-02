using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceTranslate : KdbNode {
        public override KdbNodeType Type => KdbNodeType.SourceTranslate;

        public KdbNodeSourceTranslate() : base() { }

        public KdbNodeSourceTranslate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {

        }

        protected override void DrawBody() {

        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
        ];
    }
}
