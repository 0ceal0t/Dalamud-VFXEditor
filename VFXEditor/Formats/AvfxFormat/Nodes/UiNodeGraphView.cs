using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class UiNodeGraphView : IUiItem {
        public AvfxNode Node;
        private static readonly uint BgColor = ImGui.GetColorU32( new Vector4( 0.13f, 0.13f, 0.13f, 1 ) );
        private static readonly uint BgColor2 = ImGui.GetColorU32( new Vector4( 0.3f, 0.3f, 0.3f, 1 ) );
        private static readonly uint BorderColor = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        private static readonly uint TextColor = ImGui.GetColorU32( new Vector4( 0.9f, 0.9f, 0.9f, 1 ) );
        private static readonly uint LineColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.7f, 0.7f, 1 ) );
        private static readonly Vector2 BoxSize = new( 160, 30 );
        private static readonly Vector2 Spacing = new( 200, 100 );
        private static readonly Vector2 TextOffset = new( 10, 5 );
        private static readonly Vector2 LineOffset = new( 0, 15 );

        private static readonly uint GridColor = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        private static readonly int GridSmall = 10;
        private static readonly int GridLarge = 50;

        private bool ViewDrag = false;
        private Vector2 LastDragPos;
        private Vector2 OFFSET = new( -30, -50 );

        public UiNodeGraphView( AvfxNode node ) {
            Node = node;
        }

        public void Draw() {
            if( Node.Graph == null || Node.Graph.Outdated ) {
                Node.Graph = new UiNodeGraph( Node );
            }
            if( Node.Graph.Cycle ) {
                ImGui.TextColored( UiUtils.RED_COLOR, "WARNING: CYCLICAL" );
                return;
            }

            if( !ImGui.CollapsingHeader( "Node Graph", ImGuiTreeNodeFlags.DefaultOpen ) ) return;

            // now the fun (tm) begins
            var space = ImGui.GetContentRegionAvail();
            var Size = new Vector2( space.X, 150 );
            var drawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##NodeEmpty", Size );
            var canvasTopLeft = ImGui.GetItemRectMin();
            var canvasBottomRight = ImGui.GetItemRectMax();
            drawList.PushClipRect( canvasTopLeft, canvasBottomRight, true );

            drawList.AddRectFilled( canvasTopLeft, canvasBottomRight, BgColor );

            // ========= GRID =========

            for( var i = 0; i < Size.X / GridSmall; i++ ) {
                drawList.AddLine( new Vector2( canvasTopLeft.X + i * GridSmall, canvasTopLeft.Y ), new Vector2( canvasTopLeft.X + i * GridSmall, canvasBottomRight.Y ), GridColor, 1.0f );
            }
            for( var i = 0; i < Size.Y / GridSmall; i++ ) {
                drawList.AddLine( new Vector2( canvasTopLeft.X, canvasTopLeft.Y + i * GridSmall ), new Vector2( canvasBottomRight.X, canvasTopLeft.Y + i * GridSmall ), GridColor, 1.0f );
            }
            for( var i = 0; i < Size.X / GridLarge; i++ ) {
                drawList.AddLine( new Vector2( canvasTopLeft.X + i * GridLarge, canvasTopLeft.Y ), new Vector2( canvasTopLeft.X + i * GridLarge, canvasBottomRight.Y ), GridColor, 2.0f );
            }
            for( var i = 0; i < Size.Y / GridLarge; i++ ) {
                drawList.AddLine( new Vector2( canvasTopLeft.X, canvasTopLeft.Y + i * GridLarge ), new Vector2( canvasBottomRight.X, canvasTopLeft.Y + i * GridLarge ), GridColor, 2.0f );
            }

            drawList.AddRect( canvasTopLeft, canvasBottomRight, BgColor2 );

            foreach( var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                var Pos = canvasBottomRight - GetPos( item ) + LineOffset; // left node
                foreach( var n in item.Next ) {
                    var item2 = Node.Graph.Graph[n];
                    var Pos2 = canvasBottomRight - GetPos( item2 ) + BoxSize - LineOffset; // right node

                    Vector2 Mid;
                    Vector2 Mid1;
                    Vector2 Mid2;
                    if( item2.Level - item.Level > 1 && item2.Level2 == item.Level2 ) { // SKIP
                        var Diff = Pos2.X - Pos.X;
                        Mid1 = new Vector2( Pos.X + Diff * 0.1f, Pos.Y + 50 );
                        Mid2 = new Vector2( Pos.X + Diff * 0.9f, Pos.Y + 50 );
                    }
                    else {
                        Mid = Pos + ( Pos2 - Pos ) / 2;
                        Mid1 = new Vector2( Mid.X, Pos.Y );
                        Mid2 = new Vector2( Mid.X, Pos2.Y );
                    }

                    drawList.AddBezierCubic( Pos, Mid1, Mid2, Pos2, LineColor, 3.0f );
                }
            }
            foreach( var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                var pos = canvasBottomRight - GetPos( item );
                drawList.AddRectFilled( pos, pos + BoxSize, node.GraphColor, 5 );

                drawList.AddText( pos + TextOffset, TextColor, TrimText( node.GetText() ) );

                var buttonPos = pos + new Vector2( BoxSize.X - 22, TextOffset.Y + 3 );
                var buttonOver = ImGui.IsMouseHoveringRect( canvasTopLeft, canvasBottomRight ) && ImGui.IsMouseHoveringRect( buttonPos, buttonPos + new Vector2( 20 ) );

                drawList.AddText( UiBuilder.IconFont, 12, buttonPos, buttonOver ? BgColor : 0xFFFFFFFF, FontAwesomeIcon.Share.ToIconString() );
                if( buttonOver && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.IsItemFocused() ) {
                    Plugin.AvfxManager.File.SelectItem( node ); // navigate to node
                }

                if( item.Level > 0 ) { // right node
                    var Pos2 = canvasBottomRight - GetPos( item ) + BoxSize - LineOffset;
                    drawList.AddCircleFilled( Pos2, 4, LineColor );
                    drawList.AddCircle( Pos2, 5, BorderColor );
                }
                if( item.Next.Count > 0 ) { // left node
                    var Pos2 = canvasBottomRight - GetPos( item ) + LineOffset;
                    drawList.AddCircleFilled( Pos2, 4, LineColor );
                    drawList.AddCircle( Pos2, 5, BorderColor );
                }
            }

            // HOW ABOUT DRAGGING THE VIEW?
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var d = ImGui.GetMouseDragDelta();
                if( ViewDrag ) {
                    var delta = d - LastDragPos;
                    OFFSET -= delta;
                }
                ViewDrag = true;
                LastDragPos = d;
            }
            else {
                ViewDrag = false;
            }

            drawList.PopClipRect();
            ImGui.EndGroup();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
        }

        private static string TrimText( string text ) {
            var allowedWidth = BoxSize.X - TextOffset.X - 25;
            if( ImGui.CalcTextSize( text ).X <= allowedWidth ) return text;

            var ellipWidth = ImGui.CalcTextSize( "..." ).X;
            for( var i = 0; i < text.Length; i++ ) {
                var subText = text[0..^i];
                if( ( ellipWidth + ImGui.CalcTextSize( subText ).X ) <= allowedWidth ) return subText + "...";
            }

            return "";
        }

        public Vector2 GetPos( UiNodeGraphItem item ) => new Vector2( Spacing.X * ( item.Level + 1 ), Spacing.Y * ( item.Level2 + 1 ) ) + OFFSET;
    }
}
