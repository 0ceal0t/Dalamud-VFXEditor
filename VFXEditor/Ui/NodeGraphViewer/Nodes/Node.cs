using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Commands;
using VfxEditor.Ui.NodeGraphViewer.Nodes;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer {
    public abstract class Node {
        public static readonly Vector2 NodeInsidePadding = new( 10f, 3.5f );
        public static readonly Vector2 MinHandleSize = new( 50, 20 );
        public const float EdgeThickness = 2f;
        public const float EdgeAnchorSize = 7.5f;
        public const float SlotSpacing = 25f;

        private static int NODE_ID = 0;
        public readonly int Id = NODE_ID++;

        protected bool NeedReinit = false;
        public readonly NodeStyle Style = new( Vector2.Zero, Vector2.Zero );

        protected string Name;

        public Node() { }

        public string GetText() => Name;

        public virtual void SetHeader( string header, bool pAutoSizing = true, bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            Name = header;
            Style.SetHandleTextSize( ImGui.CalcTextSize( Name ) );
            if( pAutoSizing ) AdjustSizeToHeader( pAdjustWidthOnly, pChooseGreaterWidth );
        }

        public virtual void AdjustSizeToHeader( bool pAdjustWidthOnly = false, bool pChooseGreaterWidth = false ) {
            if( pAdjustWidthOnly ) {
                var tW = ( ImGui.CalcTextSize( Name ) + NodeInsidePadding * 2 ).X;
                Style.SetSize( new( pChooseGreaterWidth ? ( tW > Style.GetSize().X ? tW : Style.GetSize().X ) : tW, Style.GetSize().Y ) );
            }
            else Style.SetSize( ImGui.CalcTextSize( Name ) + NodeInsidePadding * 2 );
        }

        protected virtual void DrawHandle( Vector2 pNodeOSP, float canvasScaling, ImDrawListPtr pDrawList, bool selected ) {
            var handleSize = Style.GetHandleSizeScaled( canvasScaling );
            pDrawList.AddRectFilled(
                pNodeOSP,
                pNodeOSP + handleSize,
                ImGui.ColorConvertFloat4ToU32( NodeUtils.AdjustTransparency( Style.ColorUnique, selected ? 0.45f : 0.15f ) ) );

            ImGui.TextColored( NodeUtils.Colors.NodeText, Name );
        }

        public void DrawEdge(
            ImDrawListPtr drawList,
            Vector2 sourcePos,
            Vector2 targetPos,
            Slot source,
            Slot target,
            bool highlighted,
            bool highlightedNegative,
            ref (Slot, Slot) slotPopup
            ) {
            var cursor = ImGui.GetCursorScreenPos();
            var anchorPosition = sourcePos + ( targetPos - sourcePos ) * 0.5f;
            var anchorSize = new Vector2( EdgeAnchorSize, EdgeAnchorSize );
            var anchorHovered = false;

            ImGui.SetCursorScreenPos( anchorPosition - anchorSize );
            ImGui.InvisibleButton( $"anchor{Id}{target.Node.Id}{source.Index}{target.Index}", anchorSize * 3f, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonMiddle );

            if( ImGui.IsItemActive() ) {
                if( ImGui.GetIO().MouseClicked[0] == true ) {
                    slotPopup = (source, target);
                    ImGui.OpenPopup( "SlotPopup" );
                }
                else if( ImGui.GetIO().MouseClicked[1] == true ) CommandManager.Add( new NodeSlotCommand( source, target, false ) ); // remove with right-click
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

        public abstract int GetOutputCount();
        public abstract List<Node> GetParents();
    }

    public abstract class Node<S> : Node where S : Slot {
        public readonly List<S> Inputs;
        public readonly List<S> Outputs;

        protected virtual Vector2 BodySize => new( 200, SlotSpacing * ( Inputs.Count + Outputs.Count ) + Style.GetHandleSize().Y );

        public Node() {
            Inputs = GetInputSlots();
            Outputs = GetOutputSlots();

            foreach( var (slot, idx) in Inputs.WithIndex() ) slot.Setup( this, idx, true );
            foreach( var (slot, idx) in Outputs.WithIndex() ) slot.Setup( this, idx, false );
        }

        public Node( string name ) : this() {
            Name = name;
            InitName();
        }

        protected void InitName() {
            if( Plugin.IsImguiSafe ) SetHeader( Name );
            else {
                Style.SetHandleTextSize( new Vector2( Name.Length * 6, 11 ) );
                Style.SetSize( new Vector2( Name.Length * 6, 11 ) + NodeInsidePadding * 2 );
                NeedReinit = true;
            }
            Style.SetSize( BodySize );
        }

        protected abstract List<S> GetInputSlots();
        protected abstract List<S> GetOutputSlots();
        public bool ChildOf( Node node ) => Inputs.Any( x => x.IsConnectedTo( node ) );

        public override List<Node> GetParents() => [.. Inputs.SelectMany( x => x.GetConnections().Select( x => x.Node ) )];
        public override int GetOutputCount() => Outputs.Count;

        protected virtual void Refresh() {
            SetHeader( Name );
            Style.SetSize( BodySize );
        }

        public void Draw( Vector2 nodePos, float canvasScaling, bool selected, Slot connection, out Slot _connection ) {
            using var _ = ImRaii.PushId( Id );

            // Re-calculate ImGui-dependant members, if required.
            if( NeedReinit ) {
                Refresh();
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

            foreach( var slot in Inputs ) slot.Draw( drawList, slot.GetSlotPosition( nodePos, canvasScaling ), canvasScaling, selected, connection, out connection );
            foreach( var slot in Outputs ) slot.Draw( drawList, slot.GetSlotPosition( nodePos, canvasScaling ), canvasScaling, selected, connection, out connection );

            _connection = connection;
        }
    }
}
