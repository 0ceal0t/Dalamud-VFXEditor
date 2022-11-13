using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class UiCurveEditor : IUiBase {
        private static readonly Vector4 POINT_COLOR = new( 1, 1, 1, 1 );
        private static readonly Vector4 LINE_COLOR = new( 0, 0.1f, 1, 1 );
        private static readonly float GRABBING_DISTANCE = 25;

        public UiCurveEditorPoint Selected = null;
        public readonly List<UiCurveEditorPoint> Points = new();

        public readonly AvfxCurve Curve;
        public List<AVFXCurveKey> Keys => Curve.Keys.Keys;

        private readonly bool Color;
        private bool DrawOnce = false;

        private bool PointDrag = false;
        private DateTime PointDragTime = DateTime.Now;
        private UiCurveEditorState PointDragState;

        public UiCurveEditor( AvfxCurve curve, bool color ) {
            Curve = curve;
            Color = color;
        }

        public void Initialize() { // Keys finished reading
            foreach( var key in Keys ) Points.Add( new UiCurveEditorPoint( this, key, Color ) );
        }

        public void Draw( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "Left-Click to select a point on the graph, Right-Click to add a new keyframe" );

            if( !DrawOnce || ImGui.Button( "Fit To Contents" + parentId ) ) {
                ImPlot.SetNextAxesToFit();
                DrawOnce = true;
            }

            if( Selected != null && !Points.Contains( Selected ) ) Selected = null;

            var wrongOrder = false;
            if( Color ) ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
            if( ImPlot.BeginPlot( $"{parentId}Plot", new Vector2( -1, 300 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, Color ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );
                if( Points.Count > 0 ) {
                    GetDrawLine( Points, Color, out var _xs, out var _ys );
                    var xs = _xs.ToArray();
                    var ys = _ys.ToArray();

                    ImPlot.SetNextLineStyle( LINE_COLOR, 2 );
                    ImPlot.PlotLine( Curve.GetAvfxName(), ref xs[0], ref ys[0], xs.Length );

                    // Draw gradient image
                    if( Color && Keys.Count > 1 ) {
                        if( Plugin.DirectXManager.GradientView.CurrentCurve != Curve ) Plugin.DirectXManager.GradientView.SetGradient( Curve );

                        var topLeft = new ImPlotPoint { x = Points[0].X, y = 1 };
                        var bottomRight = new ImPlotPoint { x = Points[^1].X, y = -1 };
                        ImPlot.PlotImage( parentId + "gradient-image", Plugin.DirectXManager.GradientView.Output, topLeft, bottomRight );
                    }

                    var idx = 0;
                    var draggingAnyPoint = false;
                    foreach( var point in Points ) {
                        if( Color ) ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels( point.GetImPlotPoint() ), Selected == point ? 15 : 10, Invert( point.ColorData ) );

                        if( ImPlot.DragPoint( idx, ref point.X, ref point.Y, Color ? new Vector4( point.ColorData, 1 ) : POINT_COLOR, Selected == point ? 12 : 7, ImPlotDragToolFlags.Delayed ) ) {
                            if( !PointDrag ) {
                                PointDrag = true;
                                PointDragState = GetState();
                            }
                            PointDragTime = DateTime.Now;

                            Selected = point;
                            point.UpdatePosition();
                            draggingAnyPoint = true;
                        }

                        if( idx > 0 && point.X < Points[idx - 1].X ) wrongOrder = true;
                        idx++;
                    }
                    if( PointDrag && !draggingAnyPoint && ( DateTime.Now - PointDragTime ).TotalMilliseconds > 200 ) {
                        PointDrag = false;
                        CommandManager.Avfx.Add( new UiCurveEditorDragCommand( this, PointDragState, GetState() ) );
                    }

                    if( !draggingAnyPoint && ImGui.IsMouseDown( ImGuiMouseButton.Left ) && IsHovering() ) {
                        var mousePos = ImGui.GetMousePos();
                        foreach( var point in Points ) {
                            if( ( ImPlot.PlotToPixels( point.GetImPlotPoint() ) - mousePos ).Length() < GRABBING_DISTANCE ) {
                                Selected = point;
                                break;
                            }
                        }
                    }
                }

                if( ImGui.IsMouseClicked( ImGuiMouseButton.Right ) && IsHovering() ) {
                    var pos = ImPlot.GetPlotMousePos();
                    var z = Color ? 1.0f : ( float )pos.y;
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => { // Insert point
                        InsertPoint( ( float )pos.x, 1, 1, z );
                    } ) );
                    UpdateColor();
                }

                ImPlot.EndPlot();
            }

            if( wrongOrder ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "POINTS ARE IN THE WRONG ORDER" );
                ImGui.SameLine();
                if( ImGui.Button( "Sort" + parentId ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => { // Sort
                        Keys.Sort( ( x, y ) => x.Time.CompareTo( y.Time ) );
                        Points.Sort( ( x, y ) => x.X.CompareTo( y.X ) );
                    } ) );
                    UpdateColor();
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            Selected?.Draw();
        }

        public void UpdateColor() {
            if( Color && Keys.Count > 1 ) {
                var sameTime = true;
                foreach( var key in Keys ) {
                    if( key.Time != Keys[0].Time ) {
                        sameTime = false;
                        break;
                    }
                }
                if( sameTime ) {
                    Keys[^1].Time++;
                    Points[^1].X++;
                }
                Plugin.DirectXManager.GradientView.SetGradient( Curve );
            }
        }

        public UiCurveEditorState GetState() {
            return new UiCurveEditorState {
                Points = Keys.Select( x => new UiCurveEditorPointState {
                    Type = x.Type,
                    Time = x.Time,
                    X = x.X,
                    Y = x.Y,
                    Z = x.Z
                } ).ToArray()
            };
        }

        public void SetState( UiCurveEditorState state ) {
            Selected = null;
            Keys.Clear();
            Points.Clear();
            foreach( var point in state.Points ) {
                var newKey = new AVFXCurveKey( point.Type, point.Time, point.X, point.Y, point.Z );
                Keys.Add( newKey );
                Points.Add( new UiCurveEditorPoint( this, newKey, Color ) );
            }
            UpdateColor();
        }

        private void InsertPoint( float time, float x, float y, float z ) {
            var insertIdx = 0;
            foreach( var p in Points ) {
                if( p.X > Math.Round( time ) ) break;
                insertIdx++;
            }
            var newKey = new AVFXCurveKey( KeyType.Linear, ( int )Math.Round( time ), x, y, z );
            Keys.Insert( insertIdx, newKey );
            Points.Insert( insertIdx, new UiCurveEditorPoint( this, newKey, Color ) );
        }

        // ==========================

        private static void GetDrawLine( List<UiCurveEditorPoint> points, bool color, out List<float> xs, out List<float> ys ) {
            xs = new List<float>();
            ys = new List<float>();

            if( points.Count > 0 ) {
                var pos = points[0].GetPosition();
                xs.Add( pos.X );
                ys.Add( pos.Y );
            }

            for( var idx = 1; idx < points.Count; idx++ ) {
                var p1 = points[idx - 1];
                var p2 = points[idx];

                if( p1.Key.Type == KeyType.Linear || color ) {
                    // p1 should already be added
                    var pos = p2.GetPosition();
                    xs.Add( pos.X );
                    ys.Add( pos.Y );
                }
                else if( p1.Key.Type == KeyType.Step ) {
                    // p1 should already be added
                    xs.Add( ( float )p2.X );
                    ys.Add( ( float )p1.Y );

                    var pos = p2.GetPosition();
                    xs.Add( pos.X );
                    ys.Add( pos.Y );
                }
                else if( p1.Key.Type == KeyType.Spline ) {
                    var p1_ = p1.GetPosition();
                    var p2_ = p2.GetPosition();
                    var midX = ( p2_.X - p1_.X ) / 2;
                    var handle1 = new Vector2( p1_.X + p1.Key.X * midX, p1_.Y );
                    var handle2 = new Vector2( p2_.X - p1.Key.Y * midX, p2_.Y );
                    for( var i = 0; i < 100; i++ ) {
                        var t = i / 99.0f;

                        var pos = Bezier( p1_, p2_, handle1, handle2, t );
                        xs.Add( pos.X );
                        ys.Add( pos.Y );
                    }
                    xs.Add( p2_.X );
                    ys.Add( p2_.Y );
                }
            }
        }

        private static bool IsHovering() {
            var mousePos = ImGui.GetMousePos();
            var topLeft = ImPlot.GetPlotPos();
            var plotSize = ImPlot.GetPlotSize();
            if( mousePos.X >= topLeft.X && mousePos.X < topLeft.X + plotSize.X && mousePos.Y >= topLeft.Y && mousePos.Y < topLeft.Y + plotSize.Y ) return true;
            return false;
        }

        private static uint Invert( Vector3 color ) => color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32( new Vector4( 0, 0, 0, 1 ) ) : ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );

        private static Vector2 Bezier( Vector2 p1_, Vector2 p2_, Vector2 handle1, Vector2 handle2, float t ) {
            var u = 1 - t;
            var w1 = u * u * u;
            var w2 = 3 * u * u * t;
            var w3 = 3 * u * t * t;
            var w4 = t * t * t;
            return new Vector2(
                w1 * p1_.X + w2 * handle1.X + w3 * handle2.X + w4 * p2_.X,
                w1 * p1_.Y + w2 * handle1.Y + w3 * handle2.Y + w4 * p2_.Y
            );
        }
    }
}
