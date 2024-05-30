using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLink : KdbNode {
        public KdbNodeEffectorEZParamLink() : base( KdbNodeType.EffectorEZParamLink ) { }

        public KdbNodeEffectorEZParamLink( BinaryReader reader ) : base( KdbNodeType.EffectorEZParamLink, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Input ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [
            new( ConnectionType.Output ),
        ];
    }
}
