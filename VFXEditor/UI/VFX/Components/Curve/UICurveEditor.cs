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
    public class UICurveEditor : UIBase {
        public AVFXCurve Curve;
        bool Color;

        Vector2 TimeVisible;
        Vector2 ValVisible;
        Vector2 DataTopLeft;

        static uint TextColor = ImGui.GetColorU32( new Vector4( 1,1,1,1 ) );
        static uint BGColor = ImGui.GetColorU32( new Vector4( 0.2f, 0.2f, 0.2f, 1 ) );
        static uint GridColor = ImGui.GetColorU32( new Vector4( 0.3f, 0.3f, 0.3f, 1 ) );
        static uint CircleColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.7f, 0.7f, 1 ) );
        static uint SelectedCircleColor = ImGui.GetColorU32( new Vector4( 0.9f, 0.9f, 0.9f, 1 ) );
        static uint LineColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.2f, 0.2f, 1 ) );
        static int GridX = 10;
        static int GridY = 1;
        static float GrabDistance = 15;

        public List<CurveEditorPoint> Points = new List<CurveEditorPoint>();

        public CurveEditorPoint SelectedPoint = null;
        CurveEditorPoint DraggingPoint = null;
        bool ViewDrag = false;
        Vector2 LastDragPos;

        public UICurveEditor( AVFXCurve curve, bool color ) {
            Curve = curve;
            Color = color;
            foreach( var k in Curve.Keys ) {
                Points.Add( new CurveEditorPoint( this, k, color:color ) );
            }
            Fit();
            DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
        }
        public void Fit() {
            if(Points.Count == 0 ) {
                TimeVisible = new Vector2( 0, 10 );
                ValVisible = new Vector2( -1, 1 );
                return;
            }
            TimeVisible = new Vector2( Points[0].canvasData.X, Points[0].canvasData.X );
            ValVisible = new Vector2( Points[0].canvasData.Y, Points[0].canvasData.Y );

            foreach( var point in Points ) {
                TimeVisible.X = Math.Min( point.canvasData.X, TimeVisible.X );
                TimeVisible.Y = Math.Max( point.canvasData.X, TimeVisible.Y );
                ValVisible.X = Math.Min( point.canvasData.Y, ValVisible.X );
                ValVisible.Y = Math.Max( point.canvasData.Y, ValVisible.Y );
            }
            TimeVisible += new Vector2(-10, 10);
            ValVisible += new Vector2( -1, 1 );
        }

        public override void Draw( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.SmallButton( "Fit##curveEditor" ) ) {
                Fit();
            }

            var space = ImGui.GetContentRegionAvail();
            Vector2 Size = new Vector2( space.X, 200 );
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##empty", Size );
            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.PushClipRect( CanvasTopLeft, CanvasBottomRight, true );
            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BGColor );

            Vector2 SizePerUnit = new Vector2(
                Size.X / ( TimeVisible.Y - TimeVisible.X ), // how large is 1 frame?
                Size.Y / ( ValVisible.Y - ValVisible.X ) // how large is 1 value?
            );

            //============= GRID ============
            int leftGrid = GridX * ( int )Math.Floor( TimeVisible.X / GridX );
            int rightGrid = GridX * ( int )Math.Ceiling( TimeVisible.Y / GridX );
            int bottomGrid = GridY * ( int )Math.Floor( ValVisible.X / GridY );
            int topGrid = GridY * ( int )Math.Ceiling( ValVisible.Y / GridY );
            for( int i = leftGrid; i < rightGrid; i += GridX ) {
                float gridPos = RealPosToCanvas( new Vector2( i, 0 ), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft ).X;
                DrawList.AddLine(
                    new Vector2( gridPos, CanvasTopLeft.Y ),
                    new Vector2( gridPos, CanvasBottomRight.Y ),
                    GridColor, 1.0f
                );
                DrawList.AddText(
                    new Vector2( gridPos + 5, CanvasTopLeft.Y ),
                    GridColor, i.ToString()
                );
            }
            for( int i = bottomGrid; i < topGrid; i += GridY ) {
                float gridPos = RealPosToCanvas( new Vector2( 0, i ), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft ).Y;
                DrawList.AddLine(
                    new Vector2( CanvasTopLeft.X, gridPos ),
                    new Vector2( CanvasBottomRight.X, gridPos ),
                    GridColor, 1.0f
                );
                DrawList.AddText(
                    new Vector2( CanvasTopLeft.X, gridPos + 5 ),
                    GridColor, i.ToString()
                );
            }
            DrawList.AddText( CanvasTopLeft + new Vector2( 2, 2 ), TextColor, "Right-Click to add a new keyframe" );

            //============ POINTS ==============
            foreach(var point in Points ) {
                point.canvasPos = RealPosToCanvas( point.canvasData, SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft );
            }
            int idx = 0;
            foreach( var point in Points ) {
                // LINES
                if(idx < (Points.Count - 1)) {
                    var nextPoint = Points[idx + 1].canvasPos;
                    if(point.Key.Type == KeyType.Step ) {
                        var stepPoint = new Vector2( nextPoint.X, point.canvasPos.Y );
                        DrawList.AddLine(
                            point.canvasPos,
                            stepPoint,
                            LineColor, 4.0f
                        );
                        DrawList.AddLine(
                            stepPoint,
                            nextPoint,
                            LineColor, 4.0f
                        );
                    }
                    else {
                        DrawList.AddLine(
                            point.canvasPos,
                            nextPoint,
                            LineColor, 4.0f
                        );
                    }
                }
                // CIRCLES
                if(SelectedPoint == point ) {
                    DrawList.AddCircleFilled( point.canvasPos, 10, SelectedCircleColor );
                }
                if( !Color ) {
                    DrawList.AddCircleFilled( point.canvasPos, 7, CircleColor );
                }
                else {
                    DrawList.AddCircleFilled( point.canvasPos, 7, point.ColorData );
                }
                // SELECT
                if(ImGui.IsAnyItemActive() && ImGui.IsMouseClicked( 0 ) ) {
                    float distance = ( point.canvasPos - ImGui.GetMousePos() ).Length();
                    if( distance < GrabDistance ) {
                        SelectedPoint = point;
                    }
                }
                // DRAG SELECT
                if( ImGui.IsItemActive() && ImGui.IsMouseDragging() ) {
                    float distance = ( point.canvasPos - ImGui.GetMousePos() ).Length();
                    if(distance < GrabDistance && DraggingPoint == null ) {
                        DraggingPoint = point;
                        LastDragPos = ImGui.GetMouseDragDelta();
                    }
                }
                idx++;
            }

            // OK, NOW WE'RE READY TO HANDLE THE DRAG
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging() && DraggingPoint != null ) {
                var d = ImGui.GetMouseDragDelta();
                DragPoint( d, SizePerUnit );
            }
            else {
                DraggingPoint = null;
            }
            // HOW ABOUT MOVING THE VIEW?
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging() && DraggingPoint == null ) {
                var d = ImGui.GetMouseDragDelta();
                DragView( d, SizePerUnit );
            }
            else {
                ViewDrag = false;
            }
            // ADD A NEW ONE
            if(ImGui.IsItemHovered() && ImGui.IsMouseClicked(1) ) {
                var pos = CanvasPosToReal( ImGui.GetMousePos(), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft );
                int insertIdx = 0;
                foreach(var p in Points ) {
                    if((int)p.canvasData.X > (int)pos.X ) {
                        break;
                    }
                    insertIdx++;
                }
                float z = Color ? 1.0f : pos.Y;
                AVFXKey newKey = new AVFXKey( KeyType.Linear, (int)pos.X, 1, 1, z );
                Curve.Keys.Insert( insertIdx, newKey );
                Points.Insert( insertIdx, new CurveEditorPoint( this, newKey, color: Color ) );
            }

            DrawList.PopClipRect();
            ImGui.EndGroup();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if(SelectedPoint != null ) {
                SelectedPoint.Draw();
            }
        }

        public void DragPoint(Vector2 delta, Vector2 scaling ) {
            var d = delta - LastDragPos;
            var dataMove = CanvasDeltaToRealDelta( d, scaling );
            dataMove.Y *= -1;
            DraggingPoint.SetCanvasData( DraggingPoint.canvasData + dataMove );
            LastDragPos = delta;
        }

        public void DragView(Vector2 delta, Vector2 scaling ) {
            if( ViewDrag ) {
                var d = delta - LastDragPos;
                var dataMove = CanvasDeltaToRealDelta( d, scaling );
                dataMove.X *= -1;
                DataTopLeft += dataMove;
                TimeVisible += new Vector2(dataMove.X);
                ValVisible += new Vector2( dataMove.Y );
            }
            ViewDrag = true;
            LastDragPos = delta;
        }

        public Vector2 RealPosToCanvas( Vector2 pos, Vector2 scaling, Vector2 canvasTopLeft, Vector2 canvasBottomRight, Vector2 dataTopLeft ) {
            Vector2 result = ( pos - dataTopLeft ) * scaling;
            return new Vector2( canvasTopLeft.X + result.X, canvasBottomRight.Y - result.Y );
        }

        public Vector2 CanvasPosToReal( Vector2 pos, Vector2 scaling, Vector2 canvasTopLeft, Vector2 canvasBottomRight, Vector2 dataTopLeft ) {
            var canvasRel = pos - canvasTopLeft;
            float canvasHeight = canvasBottomRight.Y - canvasTopLeft.Y;
            canvasRel.Y *= -1;
            canvasRel.Y += canvasHeight;
            return dataTopLeft + canvasRel / scaling;
        }

        public Vector2 CanvasDeltaToRealDelta(Vector2 delta, Vector2 scaling) {
            return delta / scaling;
        }
    }

    public class CurveEditorPoint {
        public static readonly string[] TypeOptions = Enum.GetNames( typeof( KeyType ) );
        public int TypeIdx;

        public bool Color;
        public Vector2 canvasData;
        public Vector2 canvasPos;
        public Vector3 Data;
        public AVFXKey Key;
        public UICurveEditor Curve;
        public uint ColorData;

        public CurveEditorPoint( UICurveEditor curve, AVFXKey key, bool color = false ) {
            Curve = curve;
            Key = key;
            Color = color;
            Data = new Vector3( key.X, key.Y, key.Z );
            if( !Color ) {
                canvasData = new Vector2( key.Time, key.Z );
            }
            else {
                canvasData = new Vector2( key.Time, 0 );
                SetColorData();
            }
        }

        public void SetCanvasData( Vector2 data ) {
            Key.Time = ( int )Math.Round( data.X );
            if( !Color ) {
                Key.Z = data.Y;
                Data.Z = data.Y;
                canvasData = data;
            }
            else {
                canvasData.X = data.X;
            }
        }

        public void SetColorData() {
            ColorData = ImGui.GetColorU32( new Vector4( Data, 1 ) );
        }

        public void Draw() {
            var id = "##CurveEdit";
            if( UIUtils.RemoveButton( "Delete Key" + id, small: true ) ) {
                Curve.Curve.removeKey( Key );
                Curve.Points.Remove( this );
                if(Curve.SelectedPoint == this ) {
                    Curve.SelectedPoint = null;
                }
                return;
            }
            int Time = Key.Time;
            if( ImGui.InputInt( "Time" + id, ref Time ) ) {
                Key.Time = Time;
                canvasData.X = Time;
            }
            if( UIUtils.EnumComboBox( "Type" + id, TypeOptions, ref TypeIdx ) ) {
                Enum.TryParse( TypeOptions[TypeIdx], out KeyType newKeyType );
                Key.Type = newKeyType;
            }
            //=====================
            if( Color ) {
                if( ImGui.ColorEdit3( "Color" + id, ref Data, ImGuiColorEditFlags.Float ) ) {
                    Key.X = Data.X;
                    Key.Y = Data.Y;
                    Key.Z = Data.Z;

                    SetColorData();
                }
            }
            else {
                if( ImGui.InputFloat3( "Value" + id, ref Data ) ) {
                    Key.X = Data.X;
                    Key.Y = Data.Y;
                    Key.Z = Data.Z;

                    canvasData.Y = Data.Z;
                }
            }
        }
    }
}
