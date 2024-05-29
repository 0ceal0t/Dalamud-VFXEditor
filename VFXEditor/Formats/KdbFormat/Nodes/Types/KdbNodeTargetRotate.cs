using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetRotate : KdbNode {
        public KdbNodeTargetRotate() : base( KdbNodeType.TargetRotate ) { }

        public KdbNodeTargetRotate( BinaryReader reader ) : base( KdbNodeType.TargetRotate, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
