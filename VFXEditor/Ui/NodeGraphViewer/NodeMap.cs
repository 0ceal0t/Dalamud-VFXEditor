using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class NodeMap {
        private Vector2 OffsetBase = Vector2.Zero;
        private readonly Dictionary<Node, Vector2> Data = [];
        private bool NeedOffsetInit = true;

        public NodeMap() { }

        public NodeMap( Vector2 ofsBase ) {
            OffsetBase = ofsBase;
        }

        // ========================= CANVAS =========================

        public Vector2 GetBaseOffset() => OffsetBase;

        public void AddBaseOffset( Vector2 offset ) => OffsetBase += offset;

        public void ResetBaseOffset() => OffsetBase = Vector2.Zero;

        public bool FocusOnNode( Node node, Vector2? extraOfs = null ) {
            if( !Data.TryGetValue( node, out var nodeOfsFromLocalBase ) ) return false;
            ResetBaseOffset();
            AddBaseOffset( -nodeOfsFromLocalBase + ( extraOfs ?? Vector2.Zero ) );
            return true;
        }

        // ========================= NODE =========================

        public Vector2? GetNodeScreenPos( Node node, Vector2 canvasOrigninScreenPos, float canvasScaling ) {
            var relaPos = GetNodeRelaPos( node );
            if( relaPos == null ) return null;
            return canvasOrigninScreenPos + relaPos.Value * canvasScaling;
        }

        public Vector2? GetNodeRelaPos( Node node ) => Data.TryGetValue( node, out var pos ) ? pos : null;

        public void SetNodeRelaPos( Node node, Node anchorNode, Vector2 relaDelta ) {
            if( !Data.TryGetValue( node, out var value ) || !Data.ContainsKey( anchorNode ) ) return;
            var anotherPos = GetNodeRelaPos( anchorNode );
            Data[node] = anotherPos == null ? value : ( anotherPos.Value + relaDelta );
        }

        private void MoveNodeRelaPos( Node node, Vector2 relaDelta ) => SetNodeRelaPos( node, node, relaDelta );

        public void MoveNodeRelaPos( Node node, Vector2 screenDelta, float canvasScaling ) => MoveNodeRelaPos( node, screenDelta / canvasScaling );

        public void AddNode( Node node, Vector2 relaPos ) {
            if( Data.ContainsKey( node ) ) return;
            Data[node] = relaPos;
        }

        public bool RemoveNode( Node node ) {
            return Data.Remove( node );
        }

        public bool CheckNeedInitOfs() => NeedOffsetInit;

        public void MarkUnneedInitOfs() => NeedOffsetInit = false;
    }
}
