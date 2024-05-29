using VfxEditor.Ui.NodeGraphViewer;
using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbSlot : Slot {
        public KdbSlot( Node node, string name, bool isInput ) : base( node, name, isInput ) { }
    }
}
