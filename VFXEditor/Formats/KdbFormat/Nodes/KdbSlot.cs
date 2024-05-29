using VfxEditor.Ui.NodeGraphViewer.Nodes;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbSlot : Slot {
        public readonly ConnectionType Type;
        public KdbSlot( ConnectionType type ) : base( $"{type}" ) {
            Type = type;
        }
    }
}
