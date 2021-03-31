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

        static uint BGColor = ImGui.GetColorU32( new Vector4( 0.2f, 0.2f, 0.2f, 1 ) );
        static uint GridColor = ImGui.GetColorU32( new Vector4( 0.3f, 0.3f, 0.3f, 1 ) );
        static uint CircleColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.7f, 0.7f, 1 ) );
        static uint SelectedCircleColor = ImGui.GetColorU32( new Vector4( 0.9f, 0.9f, 0.9f, 1 ) );
        static uint LineColor = ImGui.GetColorU32( new Vector4( 0.7f, 0.2f, 0.2f, 1 ) );
        static uint AltLineColor = ImGui.GetColorU32( new Vector4( 0.2f, 0.2f, 0.7f, 1 ) );
        static float GrabDistance = 25;

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
            TimeVisible += new Vector2(-2, 2);
            TimeVisible.X = Math.Max(TimeVisible.X, -1);
            ValVisible += new Vector2( -0.1f, 0.1f );
            DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
        }

        public override void Draw( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.Button( "Fit to contents##curveEditor" ) ) {
                Fit();
            }
            ImGui.SameLine();
            ImGui.BeginChild( "##CurveTime", new Vector2( 170, 25 ));
            if(ImGui.InputFloat2( "Time", ref TimeVisible ) ) {
                DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
            }
            ImGui.EndChild();
            ImGui.SameLine();
            ImGui.BeginChild( "##CurveValue", new Vector2( 170, 25 ) );
            if( ImGui.InputFloat2( "Value", ref ValVisible ) ) {
                DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
            }
            ImGui.EndChild();


            var space = ImGui.GetContentRegionAvail();
            Vector2 Size = new Vector2( space.X, 200 );
            var DrawList = ImGui.GetWindowDrawList();

            ImGui.BeginGroup();

            ImGui.InvisibleButton( "##CurveEmpty", Size );
            var CanvasTopLeft = ImGui.GetItemRectMin();
            var CanvasBottomRight = ImGui.GetItemRectMax();
            DrawList.PushClipRect( CanvasTopLeft, CanvasBottomRight, true );
            DrawList.AddRectFilled( CanvasTopLeft, CanvasBottomRight, BGColor );

            Vector2 SizePerUnit = new Vector2(
                Size.X / ( TimeVisible.Y - TimeVisible.X ), // how large is 1 frame?
                Size.Y / ( ValVisible.Y - ValVisible.X ) // how large is 1 value?
            );

            //============= GRID ============
            float GridX = (float) Math.Pow(10, Math.Floor( Math.Log10( TimeVisible.Y - TimeVisible.X ) ) ); // yeah, we're big brain now
            float GridY = ( float )Math.Pow(10, Math.Floor( Math.Log10( ValVisible.Y - ValVisible.X ) ) );

            float leftGrid = GridX * (float)Math.Floor( TimeVisible.X / GridX );
            float rightGrid = GridX * ( float )Math.Ceiling( TimeVisible.Y / GridX );
            float bottomGrid = GridY * ( float )Math.Floor( ValVisible.X / GridY );
            float topGrid = GridY * ( float )Math.Ceiling( ValVisible.Y / GridY );

            for( float i = leftGrid; i < rightGrid; i += GridX ) {
                float gridPos = RealPosToCanvas( new Vector2( i, 0 ), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft ).X;
                DrawList.AddLine(
                    new Vector2( gridPos, CanvasTopLeft.Y ),
                    new Vector2( gridPos, CanvasBottomRight.Y ),
                    GridColor, 1.0f
                );
                DrawList.AddText(
                    new Vector2( gridPos + 5, CanvasTopLeft.Y ),
                    GridColor, i.ToString("F")
                );
            }
            for( float i = bottomGrid; i < topGrid; i += GridY ) {
                float gridPos = RealPosToCanvas( new Vector2( 0, i ), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft ).Y;
                DrawList.AddLine(
                    new Vector2( CanvasTopLeft.X, gridPos ),
                    new Vector2( CanvasBottomRight.X, gridPos ),
                    GridColor, 1.0f
                );
                DrawList.AddText(
                    new Vector2( CanvasTopLeft.X, gridPos + 5 ),
                    GridColor, i.ToString("F")
                );
            }

            //============ POINTS ==============
            foreach(var point in Points ) {
                point.canvasPos = RealPosToCanvas( point.canvasData, SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft );
            }
            // ===== MAIN ======
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
                    else if(point.Key.Type == KeyType.Spline && !Color ) {
                        Vector2 startPoint = point.canvasPos;
                        Vector2 endPoint = Points[idx + 1].canvasPos;

                        float midX = (endPoint.X - startPoint.X)/ 2;
                        Vector2 handle1 = new Vector2( startPoint.X + point.Data.X * midX, startPoint.Y );
                        Vector2 handle2 = new Vector2( endPoint.X - point.Data.Y - midX, endPoint.Y );

                        DrawList.AddBezierCurve( startPoint, handle1, handle2, endPoint, LineColor, 4.0f );
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
                // HOVER
                float distance = ( point.canvasPos - ImGui.GetMousePos() ).Length();
                bool closeEnough = ( distance < GrabDistance );
                if(closeEnough && DraggingPoint == null ) { // don't want to display this when dragging stuff. it's annoying
                    ImGui.BeginTooltip();
                    ImGui.Text( "#" + idx );
                    ImGui.Text( "Time: " + point.canvasData.X.ToString("F") );
                    if( !point.Color ) {
                        ImGui.Text( "Value: " + point.canvasData.Y.ToString("F") );
                    }
                    ImGui.TextColored( new Vector4( 0, 1, 0, 1 ), point.Key.Type.ToString() );
                    ImGui.EndTooltip();
                }
                // SELECT
                if(ImGui.IsAnyItemActive() && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && closeEnough ) {
                    SelectedPoint = point;
                }
                // DRAG SELECT
                if( ImGui.IsItemActive() && ImGui.IsMouseDragging(ImGuiMouseButton.Left) && closeEnough && DraggingPoint == null ) {
                    DraggingPoint = point;
                    LastDragPos = ImGui.GetMouseDragDelta();
                }
                idx++;
            }

            // OK, NOW WE'RE READY TO HANDLE THE DRAG
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) && DraggingPoint != null ) {
                var d = ImGui.GetMouseDragDelta();
                DragPoint( d, SizePerUnit );
            }
            else {
                if(DraggingPoint != null) // snap to integer time
                    DraggingPoint.canvasData.X = DraggingPoint.Key.Time;
                DraggingPoint = null;
            }
            // HOW ABOUT DRAGGING THE VIEW?
            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) && DraggingPoint == null ) {
                var d = ImGui.GetMouseDragDelta();
                DragView( d, SizePerUnit );
            }
            else {
                ViewDrag = false;
            }
            // ADD A NEW KEYFRAME
            if(ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right) ) {
                if( Points.Count > 0 ) {
                    var pos = CanvasPosToReal( ImGui.GetMousePos(), SizePerUnit, CanvasTopLeft, CanvasBottomRight, DataTopLeft );
                    int insertIdx = 0;
                    foreach( var p in Points ) {
                        if( p.canvasData.X > Math.Round( pos.X ) ) {
                            break;
                        }
                        insertIdx++;
                    }
                    float z = Color ? 1.0f : pos.Y;
                    AVFXKey newKey = new AVFXKey( KeyType.Linear, ( int )Math.Round( pos.X ), 1, 1, z );
                    Curve.Keys.Insert( insertIdx, newKey );
                    Points.Insert( insertIdx, new CurveEditorPoint( this, newKey, color: Color ) );
                }
                else { // THE FIRST POINT
                    AVFXKey newKey = new AVFXKey( KeyType.Linear, 1, 1, 1, 1 );
                    Curve.Keys.Add( newKey );
                    Points.Add( new CurveEditorPoint( this, newKey, color: Color ) );
                    Fit();
                }
            }
            // ZOOM
            if( ImGui.IsItemHovered() ) {
                Zoom( ImGui.GetIO().MouseWheel );
            }

            DrawList.PopClipRect();
            ImGui.EndGroup();
            ImGui.Text( "Right-Click to add a new keyframe" );
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
                TimeVisible += new Vector2(dataMove.X);
                FixMinimumTime();
                ValVisible += new Vector2( dataMove.Y );
                DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
            }
            ViewDrag = true;
            LastDragPos = delta;
        }

        public void Zoom(float mouseWheel ) {
            if(mouseWheel != 0 ) {
                float factor = 1 + 0.1f * mouseWheel;
                TimeVisible = ScaleVector( TimeVisible, factor );
                FixMinimumTime();
                ValVisible = ScaleVector( ValVisible, factor );
                DataTopLeft = new Vector2( TimeVisible.X, ValVisible.X );
            }
        }
        public Vector2 ScaleVector(Vector2 vec, float factor ) {
            float diff = (vec.Y - vec.X) / 2;
            float mid = vec.X + diff;
            return new Vector2( mid - factor * diff, mid + factor * diff );
        }
        public void FixMinimumTime() {
            float leftOverhang = -1 - TimeVisible.X;
            TimeVisible += new Vector2( Math.Max( 0, leftOverhang ) );
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
            TypeIdx = Array.IndexOf( TypeOptions, Key.Type.ToString() );
            Data = new Vector3( key.X, key.Y, key.Z );
            if( !Color ) {
                canvasData = new Vector2( key.Time, key.Z );
            }
            else {
                canvasData = new Vector2( key.Time, 0 );
                SetColorData();
            }
        }

        /*public void CalcCurvePoints( CurveEditorPoint nextPoint ) {
            if( Key.Type != KeyType.Spline || Color )
                return;
            if( curvePoints == null ) {
                curvePoints = new Vector2[256];
            }

            //
            // https://en.wikipedia.org/wiki/Cubic_Hermite_spline
            // for spline, X and Y are tangents T1 and T2
            // 
            // t = (x - x_start) / (x_goal - x_start)
            // p(x) = h_00(t)*y_start          +    h_10(t)(x_goal - x_start)*T1           +         h_01(t)*y_goal     +            h_11(t)(x_goal - x_start)*T2
            // 
            Vector2 startPoint = new Vector2( 0, 0 );
            Vector2 endPoint = nextPoint.canvasData - canvasData;
            float xDiff = endPoint.X - startPoint.X;
            float T1 = 0; //Data.X;
            float T2 = 0; //Data.Y;
            for(int i = 0; i < 256; i++ ) {
                float t = i / 256.0f;

                float h00 = ( 2 * t * t * t - 3 * t * t + 1 );
                float h10 = ( t * t * t - 2 * t * t + t );
                float h01 = ( -2 * t * t * t + 3 * t * t );
                float h11 = ( t * t * t - t * t );

                float p = h00 * startPoint.Y + h10 * xDiff * T1 + h01 * endPoint.Y + h11 * xDiff * T2;
                curvePoints[i] = new Vector2(startPoint.X + xDiff * t, -p);
            }
        }*/

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
                if( Curve.SelectedPoint == this ) {
                    Curve.SelectedPoint = null;
                }
                return;
            }
            if( Curve.Points[0] != this ) {
                ImGui.SameLine();
                if(ImGui.SmallButton("<< Move Left" + id ) ) {
                    var idx = Curve.Points.IndexOf( this );
                    var t = Curve.Points[idx - 1];
                    Curve.Points[idx - 1] = this;
                    Curve.Points[idx] = t;
                }
            }
            if( Curve.Points[Curve.Points.Count - 1] != this ) {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Move Right >>" + id ) ) {
                    var idx = Curve.Points.IndexOf( this );
                    var t = Curve.Points[idx + 1];
                    Curve.Points[idx + 1] = this;
                    Curve.Points[idx] = t;
                }
            }

            int Time = Key.Time;
            if( ImGui.InputInt( "Time" + id, ref Time ) ) {
                Key.Time = Time;
                canvasData.X = Time;
            }
            if( UIUtils.EnumComboBox( "Type" + id, TypeOptions, ref TypeIdx ) ) {
                Enum.TryParse( TypeOptions[TypeIdx], out KeyType newKeyType );
                Key.Type = newKeyType;
                if( newKeyType == KeyType.Spline ) {
                }
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
