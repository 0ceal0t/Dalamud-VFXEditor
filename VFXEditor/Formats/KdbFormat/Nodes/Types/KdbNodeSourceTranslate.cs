using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeSourceTranslate : KdbNode {
        public KdbNodeSourceTranslate() : base( KdbNodeType.SourceTranslate ) { }

        public KdbNodeSourceTranslate( BinaryReader reader ) : base( KdbNodeType.SourceTranslate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
        ];
    }
}
