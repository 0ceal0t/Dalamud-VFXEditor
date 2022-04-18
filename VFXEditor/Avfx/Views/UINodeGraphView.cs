using ImGuiNET;
using System.IO;
using System.Numerics;
using imnodesNET;

namespace VFXEditor.AVFX.VFX {
    public class UINodeGraphView : UIBase {
        public UINode Node;
        private static readonly uint BGColor = ImGui.GetColorU32( new Vector4( 0.13f, 0.13f, 0.13f, 1 ) );
        private static readonly uint BGColor2 = ImGui.GetColorU32( new Vector4( 0.3f, 0.3f, 0.3f, 1 ) );
        private static readonly uint BorderColor = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        private static readonly uint TextColor = ImGui.GetColorU32( new Vector4( 0.9f, 0.9f, 0.9f, 1 ) );
        private static readonly uint NodeColor = ImGui.GetColorU32( new Vector4( 0.25f, 0.25f, 0.25f, 1 ) );
        private static readonly uint LineColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.7f, 0.7f, 1 ) );
        private static Vector2 HeaderSize = new( 160, 20 );
        private static Vector2 BoxSize = new( 160, 40 );
        private static Vector2 Spacing = new( 200, 100 );
        private static Vector2 TextOffset = new( 5, 2 );
        private static Vector2 LineOffset = new( 0, 25 );

        private static readonly uint GridColor = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        private static readonly int GridSmall = 10;
        private static readonly int GridLarge = 50;

        private bool ViewDrag = false;
        private Vector2 LastDragPos;
        private Vector2 OFFSET = new( -30, -50 );

        public UINodeGraphView( UINode node ) {
            Node = node;
        }

        public override void Draw( string parentId ) {
            if( Node.Graph == null || Node.Graph.Outdated ) {
                Node.Graph = new UINodeGraph( Node );
            }
            if( Node.Graph.Cycle ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "WARNING: CYCLICAL" );
                return;
            }

            if( !ImGui.CollapsingHeader( "Node Graph", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                return;
            }

            // now the fun (tm) begins
            var space = ImGui.GetContentRegionAvail();
            var Size = new Vector2( space.X, 150 );
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##NodeEmpty", Size );
            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.PushClipRect( CanvasTopLeft, CanvasBottomRight, true );

            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BGColor );
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

            DrawList.AddRect( CanvasTopLeft, CanvasBottomRight, BGColor2 );

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
                DrawList.AddRectFilled( Pos, Pos + BoxSize, NodeColor, 5 ); // main node
                DrawList.AddRectFilled( Pos, Pos + HeaderSize, node.Color, 5, ImDrawFlags.RoundCornersTop ); // header
                DrawList.AddText( Pos + TextOffset, TextColor, node.GetText() );

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

        public Vector2 GetPos( UINodeGraphItem item ) {
            return new Vector2( Spacing.X * ( item.Level + 1 ), Spacing.Y * ( item.Level2 + 1 ) ) + OFFSET;
        }
    }
}
