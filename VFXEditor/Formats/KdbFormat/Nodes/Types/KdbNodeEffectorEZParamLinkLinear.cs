using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorEZParamLinkLinear : KdbNode {
        public KdbNodeEffectorEZParamLinkLinear() : base( KdbNodeType.EffectorEZParamLinkLinear ) { }

        public KdbNodeEffectorEZParamLinkLinear( BinaryReader reader ) : base( KdbNodeType.EffectorEZParamLinkLinear, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
