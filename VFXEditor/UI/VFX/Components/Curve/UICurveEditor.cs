using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImPlotNET;
using ImGuizmoNET;
using VFXEditor.Data.Texture;
using VFXEditor.Data.DirectX;

namespace VFXEditor.UI.VFX {
    public class UICurveEditor : UIBase {

        public CurvePoint Selected = null;
        public AVFXCurve Curve;
        List<CurvePoint> Points;
        bool Color;

        public static readonly Vector4 POINT_COLOR = new Vector4( 1, 1, 1, 1 );
        public static readonly Vector4 LINE_COLOR = new Vector4( 0, 0.1f, 1, 1 );
        public static readonly float GRABBING_DISTANCE = 25;

        public UICurveEditor( AVFXCurve curve, bool color ) {
            Curve = curve;
            Color = color;
            Points = new List<CurvePoint>();
            foreach( var key in curve.Keys ) {
                Points.Add( new CurvePoint( this, key, Color ) );
            }
        }

        public bool DrawOnce = false;
        public override void Draw( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( !DrawOnce || ImGui.Button( "Fit To Contents" + parentId ) ) {
                ImPlot.FitNextPlotAxes( true, true );
                DrawOnce = true;
            }

            bool wrongOrder = false;
            if( Color ) {
                ImPlot.SetNextPlotLimitsY( -1, 1, ImGuiCond.Always );
            }
            if( ImPlot.BeginPlot( parentId + "Plot", "Frame", "", new Vector2( -1, 300 ), ImPlotFlags.AntiAliased | ImPlotFlags.NoMenus, ImPlotAxisFlags.None, Color ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel ) ) {
                if( Points.Count > 0 ) {
                    // ====== BACKGROUND LINE =========
                    Vector2[] line = GetDrawLine( Points, Color );
                    ImPlot.SetNextLineStyle( LINE_COLOR, 2 );
                    ImPlot.PlotLine( Curve.AVFXName, ref line[0].X, ref line[0].Y, line.Length, 0, 2 * sizeof( float ) );
                    // ====== IMAGE ============
                    if( Color && Curve.Keys.Count > 1 ) {
                        if( DirectXManager.Manager._Gradient.CurrentCurve != Curve) {
                            DirectXManager.Manager._Gradient.SetGradient( Curve );
                        }

                        var topLeft = new ImPlotPoint {
                            x = Points[0].X,
                            y = 1
                        };
                        var bottomRight = new ImPlotPoint {
                            x = Points[Points.Count - 1].X,
                            y = -1
                        };

                        ImPlot.PlotImage( parentId + "gradient-image", DirectXManager.Manager._Gradient.RenderShad.NativePointer, topLeft, bottomRight );
                    }
                    // ====== POINTS ===========
                    int idx = 0;
                    var dragging = false;
                    foreach( var p in Points ) {
                        // DRAW THE POINT
                        if( Color ) {
                            ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels(p.GetImPlotPoint()), Selected == p ? 15 : 10, Invert(p.ColorData) );
                        }
                        if( ImPlot.DragPoint( "#" + idx, ref p.X, ref p.Y, true, Color ? new Vector4( p.ColorData, 1 ) : POINT_COLOR, Selected == p ? 12 : 7 ) ) {
                            Selected = p;
                            p.UpdatePosition();
                            dragging = true;
                        }
                        // ==== CHECK ORDER ======
                        if(idx > 0 && p.X < Points[idx - 1].X ) {
                            wrongOrder = true;
                        }
                        idx++;
                    }
                    // ====== CLICKING TO SELECT ======
                    if( !dragging && ImGui.IsMouseDown( ImGuiMouseButton.Left ) && IsHovering() ) {
                        var mousePos = ImGui.GetMousePos();
                        foreach( var p in Points ) {
                            var pointPos = ImPlot.PlotToPixels( p.GetImPlotPoint() );
                            var distance = ( pointPos - mousePos ).Length();
                            if( distance < GRABBING_DISTANCE ) {
                                Selected = p;
                                break;
                            }
                        }
                    }
                }
                // ========== ADD NEW KEYFRAMES =======
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Right ) && IsHovering() ) {
                    var pos = ImPlot.GetPlotMousePos();
                    int insertIdx = 0;
                    foreach( var p in Points ) {
                        if( p.X > Math.Round( pos.x ) ) {
                            break;
                        }
                        insertIdx++;
                    }
                    float z = Color ? 1.0f : ( float )pos.y;
                    AVFXKey newKey = new AVFXKey( KeyType.Linear, ( int )Math.Round( pos.x ), 1, 1, z );
                    Curve.Keys.Insert( insertIdx, newKey );
                    Points.Insert( insertIdx, new CurvePoint( this, newKey, Color ) );
                    UpdateColor();
                }

                ImPlot.EndPlot();
            }
            ImGui.Text( "Right-Click to add a new keyframe" );
            if( wrongOrder ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "POINT ARE IN THE WRONG ORDER" );
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Selected != null ) {
                Selected.Draw();
            }
        }

        public static uint Invert(Vector3 color ) {
            return color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32(new Vector4(0,0,0,1)) : ImGui.GetColorU32( new Vector4(1,1,1,1) );
        }

        public bool IsHovering() {
            var mousePos = ImGui.GetMousePos();
            var topLeft = ImPlot.GetPlotPos();
            var plotSize = ImPlot.GetPlotSize();
            if( mousePos.X >= topLeft.X && mousePos.X < topLeft.X + plotSize.X && mousePos.Y >= topLeft.Y && mousePos.Y < topLeft.Y + plotSize.Y ) return true;
            return false;
        }

        public Vector2[] GetDrawLine( List<CurvePoint> points, bool color = false ) {
            List<Vector2> ret = new List<Vector2>();
            if( points.Count > 0 ) {
                ret.Add( points[0].GetPosition() );
            }

            for( int idx = 1; idx < points.Count; idx++ ) {
                var p1 = points[idx - 1];
                var p2 = points[idx];
                if( p1.Key.Type == KeyType.Linear || color ) {
                    // p1 should already be added
                    ret.Add( p2.GetPosition() );
                }
                else if( p1.Key.Type == KeyType.Step ) {
                    // p1 should already be added
                    ret.Add( new Vector2( ( float )p2.X, ( float )p1.Y ) );
                    ret.Add( p2.GetPosition() );
                }
                else if( p1.Key.Type == KeyType.Spline ) {
                    var p1_ = p1.GetPosition();
                    var p2_ = p2.GetPosition();
                    float midX = ( p2_.X - p1_.X ) / 2;
                    Vector2 handle1 = new Vector2( p1_.X + p1.Key.X * midX, p1_.Y );
                    Vector2 handle2 = new Vector2( p2_.X - p1.Key.Y * midX, p2_.Y );
                    for( int i = 0; i < 100; i++ ) {
                        float t = i / 99.0f;
                        ret.Add( Bezier( p1_, p2_, handle1, handle2, t ) );
                    }
                    ret.Add( p2_ );
                }
            }
            return ret.ToArray();
        }

        public static Vector2 Bezier(Vector2 p1_, Vector2 p2_, Vector2 handle1, Vector2 handle2, float t ) {
            float u = 1 - t;
            float w1 = u * u * u;
            float w2 = 3 * u * u * t;
            float w3 = 3 * u * t * t;
            float w4 = t * t * t;
            return new Vector2(
                w1 * p1_.X + w2 * handle1.X + w3 * handle2.X + w4 * p2_.X,
                w1 * p1_.Y + w2 * handle1.Y + w3 * handle2.Y + w4 * p2_.Y
            );
        }

        public void UpdateColor() {
            if(Color && Curve.Keys.Count > 1 ) {
                var sameTime = true;
                foreach(var _key in Curve.Keys ) {
                    if(_key.Time != Curve.Keys[0].Time ) {
                        sameTime = false;
                        break;
                    }
                }
                if( sameTime ) {
                    Curve.Keys[Curve.Keys.Count - 1].Time++;
                    Points[Points.Count - 1].X++;
                }
                DirectXManager.Manager._Gradient.SetGradient( Curve );
            }
        }
        // USED FOR OTHER FUNCTIONS....
        public float GetAtTime(int frame ) {
            int idx = 0;
            if(Curve.Keys.Count > 0 && frame <= Curve.Keys[0].Time ) { // before the first point
                return Curve.Keys[0].Z;
            }
            if(Curve.Keys.Count > 0 && frame >= Curve.Keys[Curve.Keys.Count - 1].Time) { // after the last one
                return Curve.Keys[Curve.Keys.Count - 1].Z;
            }

            foreach(var nextPoint in Curve.Keys ) {
                if(frame < nextPoint.Time && idx != 0 ) {
                    // .........
                    var item = Curve.Keys[idx - 1];
                    if( item.Type == KeyType.Step ) {
                        return item.Z;
                    }
                    else {
                        float t = ((float)( frame - item.Time )) / ( nextPoint.Time - item.Time );
                        if(item.Type == KeyType.Linear ) {
                            return item.Z + t * ( nextPoint.Z - item.Z );
                        }
                        else if(item.Type == KeyType.Spline ) {
                            float midX = ((float)( nextPoint.X - item.X )) / 2.0f;
                            Vector2 handle1 = new Vector2( item.Time + item.X * midX, item.Z );
                            Vector2 handle2 = new Vector2( nextPoint.Time - item.Y * midX, nextPoint.Z );
                            var b = Bezier( new Vector2( item.Time, item.Z ), new Vector2( nextPoint.Time, nextPoint.Z ), handle1, handle2, t );
                            return b.Y;
                        }
                    }

                }
                idx++;
            }
            return 0.0f;
        }

        public class CurvePoint {
            public static readonly string[] TypeOptions = Enum.GetNames( typeof( KeyType ) );
            public int TypeIdx;
            public double X;
            public double Y;
            public bool Color;
            public Vector3 ColorData;
            public AVFXKey Key;
            public UICurveEditor Editor;

            public CurvePoint( UICurveEditor editor, AVFXKey key, bool color = false ) {
                Editor = editor;
                Key = key;
                Color = color;
                TypeIdx = Array.IndexOf( TypeOptions, Key.Type.ToString() );
                if( !Color ) {
                    X = key.Time;
                    Y = key.Z;
                }
                else {
                    X = key.Time;
                    Y = 0;
                    ColorData = new Vector3( Key.X, Key.Y, Key.Z );
                }
            }
            public Vector2 GetPosition() {
                return new Vector2( ( float )X, ( float )Y );
            }
            public ImPlotPoint GetImPlotPoint() {
                var ret = new ImPlotPoint();
                ret.x = ( float )X;
                ret.y = ( float )Y;
                return ret;
            }
            public void UpdatePosition() {
                X = Math.Round( X ); // can only have integer time
                if( X < 0 ) { // can't have negative time
                    X = 0;
                }
                Key.Time = ( int )X;
                // =============
                if( !Color ) {
                    Key.Z = ( float )Y;
                }
                else {
                    Y = 0; // can't move Y for a color node
                    Editor.UpdateColor();
                }
            }
            public void UpdateColorData() {
                Key.X = ColorData.X;
                Key.Y = ColorData.Y;
                Key.Z = ColorData.Z;
                Editor.UpdateColor();
            }

            public void Draw() {
                var id = "##CurveEdit";
                if( UIUtils.RemoveButton( "Delete Key" + id, small: true ) ) {
                    Editor.Curve.removeKey( Key );
                    Editor.Points.Remove( this );
                    if( Editor.Selected == this ) {
                        Editor.Selected = null;
                    }
                    Editor.UpdateColor();
                    return;
                }
                // ===== MOVE LEFT/RIGHT =====
                if( Editor.Points[0] != this ) {
                    ImGui.SameLine();
                    if( ImGui.SmallButton( "Shift Left" + id ) ) {
                        var idx = Editor.Points.IndexOf( this );
                        var t = Editor.Points[idx - 1];
                        Editor.Points[idx - 1] = this;
                        Editor.Points[idx] = t;
                        Editor.UpdateColor();
                    }
                }
                if( Editor.Points[Editor.Points.Count - 1] != this ) {
                    ImGui.SameLine();
                    if( ImGui.SmallButton( "Shift Right" + id ) ) {
                        var idx = Editor.Points.IndexOf( this );
                        var t = Editor.Points[idx + 1];
                        Editor.Points[idx + 1] = this;
                        Editor.Points[idx] = t;
                        Editor.UpdateColor();
                    }
                }

                int Time = Key.Time;
                if( ImGui.InputInt( "Time" + id, ref Time ) ) {
                    Key.Time = Time;
                    X = Time;
                    Editor.UpdateColor();
                }
                if( UIUtils.EnumComboBox( "Type" + id, TypeOptions, ref TypeIdx ) ) {
                    Enum.TryParse( TypeOptions[TypeIdx], out KeyType newKeyType );
                    Key.Type = newKeyType;
                }
                //=====================
                if( Color ) {
                    if( ImGui.ColorEdit3( "Color" + id, ref ColorData, ImGuiColorEditFlags.Float ) ) {
                        UpdateColorData();
                    }
                }
                else {
                    Vector3 _data = new Vector3( Key.X, Key.Y, Key.Z );
                    if( ImGui.InputFloat3( "Value" + id, ref _data ) ) {
                        Key.X = _data.X;
                        Key.Y = _data.Y;
                        Key.Z = _data.Z;
                        Y = Key.Z;
                    }
                }
            }
        }
    }
}
