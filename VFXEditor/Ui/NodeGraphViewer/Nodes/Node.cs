using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Nodes;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        public static readonly Vector2 NodeInsidePadding = new( 10f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        private const float EdgeThickness = 2f;
        private const float EdgeAnchorSize = 7.5f;
        private const float SlotSpacing = 15f;

        private static int NODE_ID = 0;
        public readonly int Id = NODE_ID++;

        protected bool NeedReinit = false;

        public string Header { get; protected set; }
        public NodeStyle Style = new( Vector2.Zero, Vector2.Zero );
        protected virtual Vector2 BodySize => new( 100, SlotSpacing * Slots.Count );

        public readonly List<Slot> Slots;
        public readonly Slot OutputSlot;

        public Node( string header ) {
            Header = header;
            Slots = GetSlots();
            OutputSlot = new( this, "Output", -1, isInput: false );

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

        protected abstract List<Slot> GetSlots();

        public bool ChildOf( Node node ) => Slots.Any( x => x.Connected == node );

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

        public void Draw( Vector2 nodePos, float canvasScaling, bool selected, Slot connection, out Slot _connection ) {
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

            // Output
            foreach( var (slot, index) in Slots.WithIndex() ) {
                slot.Draw( drawList, GetInputPosition( nodePos, index ), selected, connection, out connection );
            }

            OutputSlot.Draw( drawList, GetOutputPosition( nodePos ), selected, connection, out connection );
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

        public void DrawEdge( ImDrawListPtr drawList, Vector2 sourcePos, Vector2 targetPos, Node target, int index, bool highlighted = false, bool highlightedNegative = false ) {
            var cursor = ImGui.GetCursorScreenPos();
            var anchorPosition = sourcePos + ( targetPos - sourcePos ) * 0.5f;
            var anchorSize = new Vector2( EdgeAnchorSize, EdgeAnchorSize );
            var anchorHovered = false;

            ImGui.SetCursorScreenPos( anchorPosition - anchorSize );
            ImGui.InvisibleButton( $"anchor{Id}{target.Id}{index}", anchorSize * 3f, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonMiddle );

            if( ImGui.IsItemActive() ) {
                if( ImGui.GetIO().MouseClicked[1] == true ) Slots[index].Clear();
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

        public static Vector2 GetInputPosition( Vector2 nodePosition, int index ) => nodePosition + new Vector2( 0, SlotSpacing * index + SlotSpacing );

        public Vector2 GetOutputPosition( Vector2 nodePosition ) => nodePosition + Style.GetHandleSize();

        public abstract void Dispose();
    }
}
