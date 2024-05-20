using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Canvas;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        private static int NODE_ID = 0;
        public static readonly Vector2 NodeInsidePadding = new( 10f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        private const float EdgeThickness = 2f;
        private const float EdgeAnchorSize = 7.5f;
        private const float ConnectionRadius = 5f;
        private const float ConnectionSpacing = 5f;

        public readonly int Id = NODE_ID++;

        public string Header { get; protected set; }

        public NodeStyle Style = new( Vector2.Zero, Vector2.Zero );

        protected virtual Vector2 BodySize => new( 100, ( ConnectionRadius * 2f + ConnectionSpacing ) * InputSlots );

        protected bool NeedReinit = false;

        public int InputSlots => Inputs.Count;
        public readonly List<Node> Inputs = [];

        public Node( string header, int inputSlots ) {
            Header = header;
            for( var i = 0; i < inputSlots; i++ ) Inputs.Add( null );

            // Basically SetHeader() and AdjustSizeToContent(),
            // but we need non-ImGui option for loading out of Draw()
            if( Plugin.IsImguiSafe ) SetHeader( Header );
            else {
                Style.SetHandleTextSize( new Vector2( Header.Length * 6, 11 ) );
                Style.SetSize( new Vector2( Header.Length * 6, 11 ) + NodeInsidePadding * 2 );
                NeedReinit = true;
            }

            Style.SetSize( BodySize );
        }

        public bool ChildOf( Node node ) => Inputs.Contains( node );

        protected virtual void ReInit() {
            SetHeader( Header );               // adjust minSize to new header
            Style.SetSize( BodySize );        // adjust size to the new minSize
        }

        public virtual void SetHeader( string header, bool pAutoSizing = true, bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            Header = header;
            Style.SetHandleTextSize( ImGui.CalcTextSize( Header ) );
            if( pAutoSizing ) AdjustSizeToHeader( pAdjustWidthOnly, pChooseGreaterWidth );
        }

        public virtual void AdjustSizeToHeader( bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            if( pAdjustWidthOnly ) {
                var tW = ( ImGui.CalcTextSize( Header ) + NodeInsidePadding * 2 ).X;
                Style.SetSize( new( pChooseGreaterWidth ? ( tW > Style.GetSize().X ? tW : Style.GetSize().X ) : tW, Style.GetSize().Y ) );
            }
            else Style.SetSize( ImGui.CalcTextSize( Header ) + NodeInsidePadding * 2 );
        }

        public void Draw( Vector2 nodePos, float canvasScaling, bool selected, NodeConnection connection, out NodeConnection _connection ) {
            using var _ = ImRaii.PushId( Id );

            // Re-calculate ImGui-dependant members, if required.
            if( NeedReinit ) {
                ReInit();
                NeedReinit = false;
            }

            var nodeSize = Style.GetSizeScaled( canvasScaling );
            Vector2 tOuterWindowSizeOfs = new( 15 * canvasScaling );
            var endPosition = nodePos + nodeSize;

            ImGui.SetCursorScreenPos( nodePos - tOuterWindowSizeOfs / 2 );
            ImGui.BeginChild(
                $"##outer{Id}", nodeSize + tOuterWindowSizeOfs, false,
                ImGuiWindowFlags.NoMouseInputs | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoScrollbar );
            var drawList = ImGui.GetWindowDrawList();

            ImGui.SetCursorScreenPos( nodePos );
            // outline
            drawList.AddRect(
                nodePos,
                endPosition,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorFg, selected ? 0.7f : 0.2f ) ),
                1,
                ImDrawFlags.None,
                ( selected ? 6.5f : 4f ) * canvasScaling );

            NodeUtils.PushFontScale( canvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, NodeInsidePadding * canvasScaling );
            ImGui.PushStyleVar( ImGuiStyleVar.ScrollbarSize, 1.5f );
            ImGui.PushStyleColor( ImGuiCol.ChildBg, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            ImGui.PushStyleColor( ImGuiCol.Border, NodeUtils.AdjustTransparency( Style.ColorFg, selected ? 0.7f : 0.2f ) );

            ImGui.BeginChild( $"{Id}", nodeSize, border: true, ImGuiWindowFlags.ChildWindow );
            // backdrop (leave this here so the backgrop can overwrite the child's bg)
            drawList.AddRectFilled( nodePos, endPosition, ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            DrawHandle( nodePos, canvasScaling, drawList, selected );

            ImGui.EndChild();

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            NodeUtils.PopFontScale();

            ImGui.EndChild();

            for( var i = 0; i < InputSlots; i++ ) {
                DrawConnection( drawList, nodePos, selected, true, i, connection, out connection );
            }

            // Output
            DrawConnection( drawList, nodePos, selected, false, -1, connection, out connection );
            _connection = connection;
        }

        protected virtual void DrawHandle( Vector2 pNodeOSP, float canvasScaling, ImDrawListPtr pDrawList, bool selected ) {
            var handleSize = Style.GetHandleSizeScaled( canvasScaling );
            pDrawList.AddRectFilled(
                pNodeOSP,
                pNodeOSP + handleSize,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, selected ? 0.45f : 0.15f ) ) );

            ImGui.TextColored( NodeUtils.Colors.NodeText, Header );
        }

        public void DrawConnection( ImDrawListPtr drawList, Vector2 nodePosition, bool active, bool isInput, int index, NodeConnection connection, out NodeConnection _connection ) {
            var connectionActive = connection?.Node == this && connection?.IsInput == isInput && connection?.Index == index;
            _connection = connection;

            using var _ = ImRaii.PushId( isInput ? $"Input{index}" : "Output" );

            var position = isInput ? GetInputPosition( nodePosition, index ) : GetOutputPosition( nodePosition );
            Vector2 size = new( ConnectionRadius, ConnectionRadius );
            var hovered = false;
            var cursor = ImGui.GetCursorScreenPos();
            ImGui.SetCursorScreenPos( position - ( size * 3f ) / 2f );
            if( ImGui.InvisibleButton( $"##Button", size * 3f, ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonLeft ) ) {
                if( ImGui.GetIO().MouseReleased[0] ) {
                    if( connection == null ) {
                        _connection = new() {
                            Index = index,
                            IsInput = isInput,
                            Node = this
                        };
                    }
                    else {
                        if( connection.Node != this && connection.IsInput != isInput ) { // TODO: check cycle
                            if( isInput ) Inputs[index] = connection.Node;
                            else connection.Node.Inputs[connection.Index] = this;
                        }
                        _connection = null;
                    }
                }

            }
            else if( ImGui.IsItemHovered() ) hovered = true;

            ImGui.SetCursorScreenPos( cursor );
            drawList.AddCircleFilled(
                position, size.X * ( ( hovered || connectionActive ) ? 2f : 1 ),
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency(
                    NodeUtils.Colors.NodeFg,
                    ( active || hovered || connectionActive ) ? 1f : 0.7f ) ) );
            drawList.AddCircleFilled( position, ( size.X * 0.7f ) * ( ( hovered || connectionActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( Style.ColorBg ) );
            drawList.AddCircleFilled( position, ( size.X * 0.5f ) * ( ( hovered || connectionActive ) ? 2.5f : 1 ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, ( active || connectionActive ) ? 0.55f : 0.25f ) ) );
        }

        public void DrawEdge( ImDrawListPtr drawList, Vector2 sourcePos, Vector2 targetPos, Node target, int index, bool highlighted = false, bool highlightedNegative = false ) {
            var cursor = ImGui.GetCursorScreenPos();
            var anchorPosition = sourcePos + ( targetPos - sourcePos ) * 0.5f;
            var anchorSize = new Vector2( EdgeAnchorSize, EdgeAnchorSize );
            var anchorHovered = false;

            ImGui.SetCursorScreenPos( anchorPosition - anchorSize );
            ImGui.InvisibleButton( $"anchor{Id}{target.Id}{index}", anchorSize * 3f, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonMiddle );

            if( ImGui.IsItemActive() ) {
                if( ImGui.GetIO().MouseClicked[1] == true ) Inputs[index] = null;
            }
            else {
                anchorHovered = ImGui.IsItemHovered();
            }

            var color = ( highlightedNegative && !highlighted ) ? NodeUtils.Colors.NodeEdgeHighlightNeg : NodeUtils.Colors.NodeFg;
            ImGui.SetCursorScreenPos( cursor );

            drawList.AddBezierCubic(
                sourcePos,
                new( anchorPosition.X, sourcePos.Y ),
                new( anchorPosition.X, targetPos.Y ),
                targetPos,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( color, highlighted || highlightedNegative ? ( highlightedNegative ? 0.55f : 1 ) : 0.5f ) ),
                ( highlighted || highlightedNegative ) ? EdgeThickness * 1.4f : EdgeThickness
            );
            drawList.AddCircleFilled( anchorPosition, EdgeAnchorSize * ( anchorHovered ? 0.6f : 0.4f ), ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( color, anchorHovered ? 0.5f * 1.25f : 0.5f ) ) );
        }

        public static Vector2 GetInputPosition( Vector2 nodePosition, int index ) => nodePosition + new Vector2( 0, ( ConnectionRadius * 2f + ConnectionSpacing ) * index + ConnectionRadius );

        public Vector2 GetOutputPosition( Vector2 nodePosition ) => nodePosition + Style.GetHandleSize();

        public abstract void Dispose();
    }
}
