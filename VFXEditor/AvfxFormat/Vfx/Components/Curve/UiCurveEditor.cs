using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCurveEditor : IUiBase {
        private CurvePoint Selected = null;
        private readonly AVFXCurve Curve;
        private readonly List<AVFXCurveKey> SourceKeys;
        private readonly List<CurvePoint> Points;
        private readonly bool Color;
        private bool DrawOnce = false;

        private static readonly Vector4 POINT_COLOR = new( 1, 1, 1, 1 );
        private static readonly Vector4 LINE_COLOR = new( 0, 0.1f, 1, 1 );
        private static readonly float GRABBING_DISTANCE = 25;

        public UiCurveEditor( AVFXCurve curve, bool color ) {
            Curve = curve;
            SourceKeys = Curve.Keys.Keys;
            Color = color;

            Points = new List<CurvePoint>();
            foreach( var key in SourceKeys ) {
                Points.Add( new CurvePoint( this, key, Color ) );
            }
        }

        public void DrawInline( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.Text( "Left-Click to select a point on the graph, Right-Click to add a new keyframe" );

            if( !DrawOnce || ImGui.Button( "Fit To Contents" + parentId ) ) {
                ImPlot.SetNextAxesToFit();
                DrawOnce = true;
            }

            ImGui.SameLine();
            if( UiUtils.DisabledButton( "Copy" + parentId, SourceKeys.Count > 0 ) ) {
                CopyManager.ClearCurveKeys();
                foreach( var key in SourceKeys ) {
                    CopyManager.AddCurveKey( key.Time, key.X, key.Y, key.Z );
                }
            }

            ImGui.SameLine();
            if( UiUtils.DisabledButton( "Paste" + parentId, CopyManager.HasCurveKeys() ) ) {
                foreach( var key in CopyManager.CurveKeys ) {
                    InsertPoint( key.X, key.Y, key.Z, key.W );
                    UpdateColor();
                }
            }

            var wrongOrder = false;
            if( Color ) ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
            if( ImPlot.BeginPlot( $"{parentId}Plot", new Vector2( -1, 300 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, Color ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );
                if( Points.Count > 0 ) {
                    GetDrawLine( Points, Color, out var _xs, out var _ys );
                    var xs = _xs.ToArray();
                    var ys = _ys.ToArray();

                    ImPlot.SetNextLineStyle( LINE_COLOR, 2 );

                    ImPlot.PlotLine( Curve.GetName(), ref xs[0], ref ys[0], xs.Length );

                    // ====== IMAGE ============
                    if( Color && SourceKeys.Count > 1 ) {
                        if( VfxEditor.DirectXManager.GradientView.CurrentCurve != Curve ) {
                            VfxEditor.DirectXManager.GradientView.SetGradient( Curve );
                        }

                        var topLeft = new ImPlotPoint {
                            x = Points[0].X,
                            y = 1
                        };
                        var bottomRight = new ImPlotPoint {
                            x = Points[^1].X,
                            y = -1
                        };

                        ImPlot.PlotImage( parentId + "gradient-image", VfxEditor.DirectXManager.GradientView.Output, topLeft, bottomRight );
                    }

                    // ====== POINTS ===========
                    var idx = 0;
                    var dragging = false;
                    foreach( var p in Points ) {
                        // Draw the point
                        if( Color ) {
                            ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels( p.GetImPlotPoint() ), Selected == p ? 15 : 10, Invert( p.ColorData ) );
                        }
                        if( ImPlot.DragPoint(idx, ref p.X, ref p.Y, Color ? new Vector4( p.ColorData, 1 ) : POINT_COLOR, Selected == p ? 12 : 7, ImPlotDragToolFlags.Delayed ) ) {
                            Selected = p;
                            p.UpdatePosition();
                            dragging = true;
                        }
                        // Check order
                        if( idx > 0 && p.X < Points[idx - 1].X ) {
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
                    var z = Color ? 1.0f : ( float )pos.y;
                    InsertPoint( ( float )pos.x, 1, 1, z );
                    UpdateColor();
                }

                ImPlot.EndPlot();
            }

            if( wrongOrder ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "POINTS ARE IN THE WRONG ORDER" );
                ImGui.SameLine();
                if( ImGui.Button( "Sort" + parentId ) ) {
                    SourceKeys.Sort( ( x, y ) => x.Time.CompareTo( y.Time ) );
                    Points.Sort( ( x, y ) => x.X.CompareTo( y.X ) );
                    UpdateColor();
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( Selected != null ) {
                Selected.Draw();
            }
        }

        private void InsertPoint( float time, float x, float y, float z ) {
            var insertIdx = 0;
            foreach( var p in Points ) {
                if( p.X > Math.Round( time ) ) {
                    break;
                }
                insertIdx++;
            }
            var newKey = new AVFXCurveKey( KeyType.Linear, ( int )Math.Round( time ), x, y, z );
            SourceKeys.Insert( insertIdx, newKey );
            Points.Insert( insertIdx, new CurvePoint( this, newKey, Color ) );
        }

        public void UpdateColor() {
            if( Color && SourceKeys.Count > 1 ) {
                var sameTime = true;
                foreach( var _key in SourceKeys ) {
                    if( _key.Time != SourceKeys[0].Time ) {
                        sameTime = false;
                        break;
                    }
                }
                if( sameTime ) {
                    SourceKeys[^1].Time++;
                    Points[^1].X++;
                }
                VfxEditor.DirectXManager.GradientView.SetGradient( Curve );
            }
        }

        private static void GetDrawLine( List<CurvePoint> points, bool color, out List<float> xs, out List<float> ys ) {
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

        private static uint Invert( Vector3 color ) {
            return color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32( new Vector4( 0, 0, 0, 1 ) ) : ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );
        }

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

        private class CurvePoint {
            public static readonly KeyType[] KeyTypeOptions = ( KeyType[] )Enum.GetValues( typeof( KeyType ) );

            public double X;
            public double Y;
            public bool Color;
            public Vector3 ColorData;
            public AVFXCurveKey Key;
            public UiCurveEditor Editor;

            public CurvePoint( UiCurveEditor editor, AVFXCurveKey key, bool color = false ) {
                Editor = editor;
                Key = key;
                Color = color;
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
                var ret = new ImPlotPoint {
                    x = ( float )X,
                    y = ( float )Y
                };
                return ret;
            }

            public void UpdatePosition() {
                X = Math.Round( X ); // can only have integer time
                if( X < 0 ) { // can't have negative time
                    X = 0;
                }
                Key.Time = ( int )X;

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
                if( UiUtils.RemoveButton( "Delete Key" + id, small: true ) ) {
                    Editor.Curve.Keys.Remove( Key );
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
                if( Editor.Points[^1] != this ) {
                    ImGui.SameLine();
                    if( ImGui.SmallButton( "Shift Right" + id ) ) {
                        var idx = Editor.Points.IndexOf( this );
                        var t = Editor.Points[idx + 1];
                        Editor.Points[idx + 1] = this;
                        Editor.Points[idx] = t;
                        Editor.UpdateColor();
                    }
                }

                var Time = Key.Time;
                if( ImGui.InputInt( "Time" + id, ref Time ) ) {
                    Key.Time = Time;
                    X = Time;
                    Editor.UpdateColor();
                }

                if (UiUtils.EnumComboBox( "Type" + id, KeyTypeOptions, Key.Type, out var newKeyType ) ) {
                    Key.Type = newKeyType;
                }

                if( Color ) {
                    if( ImGui.ColorEdit3( "Color" + id, ref ColorData, ImGuiColorEditFlags.Float ) ) {
                        UpdateColorData();
                    }
                }
                else {
                    var _data = new Vector3( Key.X, Key.Y, Key.Z );
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
