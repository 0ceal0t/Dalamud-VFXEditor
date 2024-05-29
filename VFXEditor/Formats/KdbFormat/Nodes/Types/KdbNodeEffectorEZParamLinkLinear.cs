using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLinkLinear : KdbNode {
        public KdbNodeEffectorEZParamLinkLinear() : base( KdbNodeType.EffectorEZParamLinkLinear ) { }

        public KdbNodeEffectorEZParamLinkLinear( BinaryReader reader ) : base( KdbNodeType.EffectorEZParamLinkLinear, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
