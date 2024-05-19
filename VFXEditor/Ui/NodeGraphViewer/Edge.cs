using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using QuickGraph;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class Edge {
        private const float Thickness = 2f;
        private const float AnchorSize = 7.5f;

        public readonly string SourceNodeId;
        public readonly string TargetNodeId;
        public readonly SEdge<int> QgEdge;

        public Edge( string pSourceNodeId, string pTargetNodeId, SEdge<int> pQgEdge ) {
            SourceNodeId = pSourceNodeId;
            TargetNodeId = pTargetNodeId;
            QgEdge = pQgEdge;
        }

        public bool EitherWith( string pNodeId ) => StartsWith( pNodeId ) || EndsWith( pNodeId );

        public bool StartsWith( string pSourceNodeId ) => SourceNodeId == pSourceNodeId;

        public bool EndsWith( string pTargetNodeId ) => TargetNodeId == pTargetNodeId;

        public bool BothWith( string pSourceNodeId, string pTargetNodeId )
            => StartsWith( pSourceNodeId ) && EndsWith( pTargetNodeId );

        public NodeInteractionFlags Draw( ImDrawListPtr drawList, Vector2 sourcePos, Vector2 targetPos, bool highlighted = false, bool highlightedNegative = false, bool targetPacked = false ) {
            var cursor = ImGui.GetCursorScreenPos();
            var res = NodeInteractionFlags.None;

            // Anchor stuff
            var anchorPosition = sourcePos + ( targetPos - sourcePos ) * 0.5f;
            var anchorSize = new Vector2( AnchorSize, AnchorSize );
            var anchorHovered = false;

            // Anchor button (behaviour)
            ImGui.SetCursorScreenPos( anchorPosition - anchorSize );
            ImGui.InvisibleButton( $"ea{SourceNodeId}{TargetNodeId}", anchorSize * 3f, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonMiddle );

            if( ImGui.IsItemActive() ) {
                res |= NodeInteractionFlags.Edge;
                if( ImGui.GetIO().MouseClicked[1] == true ) ImGui.OpenPopup( $"##epu{SourceNodeId}{TargetNodeId}" );
                else if( ImGui.GetIO().MouseClicked[2] == true ) res |= NodeInteractionFlags.RequestEdgeRemoval;
            }
            else {
                anchorHovered = NodeUtils.SetTooltipForLastItem( $"[Left-click + drag] diagonally to switch between 3 paths: Upper or Lower perpendicular, or Diagonal.\n[Right-click] or [Middle-click] to delete." );
            }

            res |= DrawPU();

            var color = targetPacked
                            ? NodeUtils.Colors.NodePack
                            : ( highlightedNegative && !highlighted )
                                ? NodeUtils.Colors.NodeEdgeHighlightNeg
                                : NodeUtils.Colors.NodeFg;
            ImGui.SetCursorScreenPos( cursor );

            drawList.AddBezierCubic(
                sourcePos,
                new( anchorPosition.X, sourcePos.Y ),
                new( anchorPosition.X, targetPos.Y ),
                targetPos,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( color, highlighted || highlightedNegative ? ( highlightedNegative ? 0.55f : 1 ) : 0.5f ) ),
                ( highlighted || highlightedNegative ) ? Thickness * 1.4f : Thickness
            );
            drawList.AddCircleFilled( anchorPosition, AnchorSize * 0.4f, ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( color, anchorHovered ? 0.5f * 1.25f : 0.5f ) ) );
            return res;
        }

        public NodeInteractionFlags DrawPU() {
            var res = NodeInteractionFlags.None;
            if( ImGui.BeginPopup( $"##epu{SourceNodeId}{TargetNodeId}" ) ) {
                if( ImGuiComponents.IconButton( FontAwesomeIcon.Trash ) ) res |= NodeInteractionFlags.RequestEdgeRemoval;
                ImGui.EndPopup();
            }
            return res;
        }
    }
}
