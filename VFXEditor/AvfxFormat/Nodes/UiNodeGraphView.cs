using Dalamud.Interface;
using ImGuiNET;
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
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##NodeEmpty", Size );
            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.PushClipRect( CanvasTopLeft, CanvasBottomRight, true );

            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BgColor );

            // ========= GRID =========

            for( var i = 0; i < Size.X / GridSmall; i++ ) {
                DrawList.AddLine( new Vector2( CanvasTopLeft.X + i * GridSmall, CanvasTopLeft.Y ), new Vector2( CanvasTopLeft.X + i * GridSmall, CanvasBottomRight.Y ), GridColor, 1.0f );
            }
            for( var i = 0; i < Size.Y / GridSmall; i++ ) {
                DrawList.AddLine( new Vector2( CanvasTopLeft.X, CanvasTopLeft.Y + i * GridSmall ), new Vector2( CanvasBottomRight.X, CanvasTopLeft.Y + i * GridSmall ), GridColor, 1.0f );
            }
            for( var i = 0; i < Size.X / GridLarge; i++ ) {
                DrawList.AddLine( new Vector2( CanvasTopLeft.X + i * GridLarge, CanvasTopLeft.Y ), new Vector2( CanvasTopLeft.X + i * GridLarge, CanvasBottomRight.Y ), GridColor, 2.0f );
            }
            for( var i = 0; i < Size.Y / GridLarge; i++ ) {
                DrawList.AddLine( new Vector2( CanvasTopLeft.X, CanvasTopLeft.Y + i * GridLarge ), new Vector2( CanvasBottomRight.X, CanvasTopLeft.Y + i * GridLarge ), GridColor, 2.0f );
            }

            DrawList.AddRect( CanvasTopLeft, CanvasBottomRight, BgColor2 );

            foreach( var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                var Pos = CanvasBottomRight - GetPos( item ) + LineOffset; // left node
                foreach( var n in item.Next ) {
                    var item2 = Node.Graph.Graph[n];
                    var Pos2 = CanvasBottomRight - GetPos( item2 ) + BoxSize - LineOffset; // right node

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

                    DrawList.AddBezierCubic( Pos, Mid1, Mid2, Pos2, LineColor, 3.0f );
                }
            }
            foreach( var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                var Pos = CanvasBottomRight - GetPos( item );
                DrawList.AddRectFilled( Pos, Pos + BoxSize, node.GraphColor, 5 );

                DrawList.AddText( Pos + TextOffset, TextColor, TrimText( node.GetText() ) );

                var buttonPos = Pos + new Vector2( BoxSize.X - 22, TextOffset.Y + 3 );
                var buttonOver = UiUtils.MouseOver( CanvasTopLeft, CanvasBottomRight ) && UiUtils.MouseOver( buttonPos, buttonPos + new Vector2( 20 ) );

                DrawList.AddText( UiBuilder.IconFont, 12, buttonPos, buttonOver ? BgColor : 0xFFFFFFFF, $"{( char )FontAwesomeIcon.Share}" );
                if( buttonOver && UiUtils.MouseClicked() ) {
                    Plugin.AvfxManager.CurrentFile.SelectItem( node ); // navigate to node
                }

                if( item.Level > 0 ) { // right node
                    var Pos2 = CanvasBottomRight - GetPos( item ) + BoxSize - LineOffset;
                    DrawList.AddCircleFilled( Pos2, 4, LineColor );
                    DrawList.AddCircle( Pos2, 5, BorderColor );
                }
                if( item.Next.Count > 0 ) { // left node
                    var Pos2 = CanvasBottomRight - GetPos( item ) + LineOffset;
                    DrawList.AddCircleFilled( Pos2, 4, LineColor );
                    DrawList.AddCircle( Pos2, 5, BorderColor );
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

            DrawList.PopClipRect();
            ImGui.EndGroup();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
        }

        private static string TrimText( string text ) => text.Length <= 20 ? text : text[..20] + "...";

        public Vector2 GetPos( UiNodeGraphItem item ) => new Vector2( Spacing.X * ( item.Level + 1 ), Spacing.Y * ( item.Level2 + 1 ) ) + OFFSET;
    }
}
