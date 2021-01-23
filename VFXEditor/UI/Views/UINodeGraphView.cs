using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UINodeGraphView : UIBase {
        public UINode Node;

        static uint BGColor = ImGui.GetColorU32( new Vector4( 0.1f, 0.1f, 0.1f, 1 ) );
        static uint BGColor2 = ImGui.GetColorU32( new Vector4( 0.3f, 0.3f, 0.3f, 1 ) );
        static uint TextColor = ImGui.GetColorU32( new Vector4( 0.0f, 0.0f, 0.0f, 1 ) );
        static uint CircleColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.7f, 0.7f, 1 ) );
        static uint LineColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.2f, 0.2f, 1 ) );

        static Vector2 BoxSize = new Vector2( 160, 40 );
        static Vector2 Spacing = new Vector2( 200, 100);
        static Vector2 TextOffset = new Vector2( 10, 10 );
        static Vector2 LineOffset = new Vector2( 0, 20 );

        bool ViewDrag = false;
        Vector2 LastDragPos;
        Vector2 OFFSET = new Vector2( -30, -50 );

        public UINodeGraphView(UINode node ) {
            Node = node;
        }

        public override void Draw( string parentId ) {
            if(Node.Graph == null || Node.Graph.Outdated ) {
                Node.Graph = new UINodeGraph( Node );
            }
            if( Node.Graph.Cycle ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "WARNING: CYCLICAL" );
                return;
            }
            // now the fun (tm) begins
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var space = ImGui.GetContentRegionAvail();
            Vector2 Size = new Vector2( space.X, 150 );
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##NodeEmpty", Size );
            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.PushClipRect( CanvasTopLeft, CanvasBottomRight, true );

            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BGColor );
            DrawList.AddRect( CanvasTopLeft, CanvasBottomRight, BGColor2 );

            foreach( var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                Vector2 Pos = CanvasBottomRight - GetPos( item ) + LineOffset;
                foreach(var n in item.Next ) {
                    var item2 = Node.Graph.Graph[n];
                    Vector2 Pos2 = CanvasBottomRight - GetPos( item2 ) + BoxSize - LineOffset;

                    Vector2 Mid;
                    Vector2 Mid1;
                    Vector2 Mid2;
                    if(item2.Level - item.Level > 1 && item2.Level2 == item.Level2) { // SKIP
                        float Diff = Pos2.X - Pos.X;
                        Mid1 = new Vector2(Pos.X + Diff * 0.1f, Pos.Y + 50);
                        Mid2 = new Vector2( Pos.X + Diff * 0.9f, Pos.Y + 50 );
                    }
                    else {
                        Mid = Pos + ( Pos2 - Pos ) / 2;
                        Mid1 = new Vector2( Mid.X, Pos.Y );
                        Mid2 = new Vector2( Mid.X, Pos2.Y );
                    }

                    DrawList.AddBezierCurve( Pos, Mid1, Mid2, Pos2, LineColor, 6.0f );
                }
            }
            foreach(var node in Node.Graph.Graph.Keys ) {
                var item = Node.Graph.Graph[node];
                Vector2 Pos = CanvasBottomRight - GetPos(item);
                DrawList.AddRectFilled( Pos, Pos + BoxSize, CircleColor, 5 );
                DrawList.AddText( Pos + TextOffset, TextColor, node.GetText() );
            }

            // HOW ABOUT DRAGGING THE VIEW?
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging()) {
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

        public Vector2 GetPos(UINodeGraphItem item ) {
            return new Vector2( Spacing.X * ( item.Level + 1 ), Spacing.Y * ( item.Level2 + 1 ) ) + OFFSET;
        }
    }
}
