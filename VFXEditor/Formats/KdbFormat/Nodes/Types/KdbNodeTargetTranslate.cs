using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetTranslate : KdbNode {
        public KdbNodeTargetTranslate() : base( KdbNodeType.TargetTranslate ) { }

        public KdbNodeTargetTranslate( BinaryReader reader ) : base( KdbNodeType.TargetTranslate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
