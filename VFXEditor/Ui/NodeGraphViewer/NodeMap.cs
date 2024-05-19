using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class NodeMap {
        private Vector2 OffsetBase = Vector2.Zero;
        private readonly Dictionary<string, Vector2> Data = [];
        private readonly HashSet<string> Keys = [];
        private bool NeedOffsetInit = true;

        public NodeMap() { }

        public NodeMap( Vector2 ofsBase ) {
            OffsetBase = ofsBase;
        }

        // ========================= CANVAS =========================

        public Vector2 GetBaseOffset() => OffsetBase;

        public void AddBaseOffset( Vector2 offset ) => OffsetBase += offset;

        public void ResetBaseOffset() => OffsetBase = Vector2.Zero;

        public bool FocusOnNode( string nodeId, Vector2? extraOfs = null ) {
            if( !Data.TryGetValue( nodeId, out var nodeOfsFromLocalBase ) ) return false;
            ResetBaseOffset();
            AddBaseOffset( -nodeOfsFromLocalBase + ( extraOfs ?? Vector2.Zero ) );
            return true;
        }

        // ========================= NODE =========================

        public Vector2? GetNodeScreenPos( string nodeId, Vector2 canvasOrigninScreenPos, float canvasScaling ) {
            var relaPos = GetNodeRelaPos( nodeId );
            if( relaPos == null ) return null;
            return canvasOrigninScreenPos + relaPos.Value * canvasScaling;
        }

        public Vector2? GetNodeRelaPos( string nodeId ) {
            if( Data.TryGetValue( nodeId, out var pos ) )
                return pos;
            return null;
        }

        public void SetNodeRelaPos( string nodeId, Vector2 relaPos ) {
            if( !Keys.Contains( nodeId ) ) return;
            Data[nodeId] = relaPos;
        }

        public void SetNodeRelaPos( string nodeId, string anchorNodeId, Vector2 relaDelta ) {
            if( !Keys.Contains( nodeId ) || !Keys.Contains( anchorNodeId ) ) return;
            var anotherPos = GetNodeRelaPos( anchorNodeId );
            Data[nodeId] = anotherPos == null ? Data[nodeId] : ( anotherPos.Value + relaDelta );
        }

        private void MoveNodeRelaPos( string nodeId, Vector2 relaDelta ) => SetNodeRelaPos( nodeId, nodeId, relaDelta );

        public void MoveNodeRelaPos( string nodeId, Vector2 screenDelta, float canvasScaling ) => MoveNodeRelaPos( nodeId, screenDelta / canvasScaling );

        public void AddNode( string nodeId, Vector2 relaPos ) {
            if( Keys.Contains( nodeId ) ) return;
            Keys.Add( nodeId );
            SetNodeRelaPos( nodeId, relaPos );
        }

        public bool RemoveNode( string nodeId ) {
            return Data.Remove( nodeId ) & Keys.Remove( nodeId );
        }

        public bool CheckNodeExist( string nodeId ) => Keys.Contains( nodeId );

        public bool CheckNeedInitOfs() => NeedOffsetInit;

        public void MarkUnneedInitOfs() => NeedOffsetInit = false;
    }
}
