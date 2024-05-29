using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLink : KdbNode {
        public KdbNodeEffectorEZParamLink() : base( KdbNodeType.EffectorExpr ) { }

        public KdbNodeEffectorEZParamLink( BinaryReader reader ) : base( KdbNodeType.EffectorExpr, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
