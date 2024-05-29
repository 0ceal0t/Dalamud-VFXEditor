using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeEffectorExpr : KdbNode {
        public KdbNodeEffectorExpr() : base( KdbNodeType.EffectorEZParamLink ) { }

        public KdbNodeEffectorExpr( BinaryReader reader ) : base( KdbNodeType.EffectorEZParamLink, reader ) { }

        public override void ReadBody( BinaryReader reader ) { }

        protected override List<Slot> GetInputSlots() => [];

        protected override List<Slot> GetOutputSlots() => [];
    }
}
