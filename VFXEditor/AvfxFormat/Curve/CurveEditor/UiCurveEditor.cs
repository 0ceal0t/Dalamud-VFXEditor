using Dalamud.Logging;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditor : IAvfxUiBase {
        private static int EditorCount = 0;

        public readonly List<UiCurveEditorPoint> Selected = new();
        public UiCurveEditorPoint PrimarySelected => Selected.Count == 0 ? null : Selected[0];
        public readonly List<UiCurveEditorPoint> Points = new();

        public readonly AvfxCurve Curve;
        public List<AvfxCurveKey> Keys => Curve.Keys.Keys;

        private readonly int EditorId;
        private readonly CurveType Type;
        private bool IsColor => Type == CurveType.Color;
        private bool DrawOnce = false;

        private bool IsPointDragged = false;
        private DateTime PointDragStartTime = DateTime.Now;
        private UiCurveEditorState PrePointDragState;

        public UiCurveEditor( AvfxCurve curve, CurveType type ) {
            Curve = curve;
            Type = type;
            EditorId = EditorCount++;
        }

        public void Initialize() { // Keys finished reading
            foreach( var key in Keys ) Points.Add( new UiCurveEditorPoint( this, key, Type ) );
        }

        public void Draw( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.TextDisabled( "Curve editor controls (?)" );
            UiUtils.Tooltip( "Ctrl + left-click to add a new point\nLeft-click to select a point, hold shift to select multiple\nWith multiple points selected, hold shift when dragging to move all of them" );

            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) );

            if( !DrawOnce || ImGui.SmallButton( "Fit To Contents" + parentId ) ) {
                ImPlot.SetNextAxesToFit();
                DrawOnce = true;
            }

            ImGui.SameLine();
            if( UiUtils.DisabledButton( "Copy" + parentId, Keys.Count > 0, true ) ) {
                CopyManager.Avfx.ClearCurveKeys();
                foreach( var key in Keys ) CopyManager.Avfx.AddCurveKey( key.Time, key.X, key.Y, key.Z );
            }

            ImGui.SameLine();
            if( UiUtils.DisabledButton( "Paste" + parentId, CopyManager.Avfx.HasCurveKeys(), true ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => {
                    foreach( var key in CopyManager.Avfx.CurveKeys ) InsertPoint( key.X, key.Y, key.Z, key.W );
                    UpdateGradient();
                } ) );
            }

            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Clear" + parentId, true ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => {
                    Points.Clear();
                    Keys.Clear();
                    UpdateGradient();
                    Selected.Clear();
                } ) );
            }

            ImGui.PopStyleVar( 1 );

            // =================

            if( Type == CurveType.Angle ) {
                if( ImGui.RadioButton( $"Radians##{parentId}1", !Plugin.Configuration.UseDegreesForAngles ) ) {
                    Plugin.Configuration.UseDegreesForAngles = false;
                    Plugin.Configuration.Save();
                }
                ImGui.SameLine();
                if( ImGui.RadioButton( $"Degrees##{parentId}2", Plugin.Configuration.UseDegreesForAngles ) ) {
                    Plugin.Configuration.UseDegreesForAngles = true;
                    Plugin.Configuration.Save();
                }
            }

            Selected.RemoveAll( x => !Points.Contains( x ) );

            var wrongOrder = false;
            if( IsColor ) ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
            if( ImPlot.BeginPlot( $"{parentId}-Plot-{EditorId}", new Vector2( -1, 300 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, IsColor ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );
                if( Points.Count > 0 ) {
                    GetDrawLine( Points, IsColor, out var _xs, out var _ys );
                    var xs = _xs.ToArray();
                    var ys = _ys.ToArray();

                    ImPlot.SetNextLineStyle( Plugin.Configuration.CurveEditorLineColor, Plugin.Configuration.CurveEditorLineWidth );
                    ImPlot.PlotLine( Curve.GetAvfxName(), ref xs[0], ref ys[0], xs.Length );

                    // Draw gradient image
                    if( IsColor && Keys.Count > 1 ) {
                        if( Plugin.DirectXManager.GradientView.CurrentCurve != Curve ) Plugin.DirectXManager.GradientView.SetGradient( Curve );

                        var topLeft = new ImPlotPoint { x = Points[0].DisplayX, y = 1 };
                        var bottomRight = new ImPlotPoint { x = Points[^1].DisplayX, y = -1 };
                        ImPlot.PlotImage( parentId + "gradient-image", Plugin.DirectXManager.GradientView.Output, topLeft, bottomRight );
                    }

                    var idx = 0;
                    var draggingAnyPoint = false;
                    foreach( var point in Points ) {
                        var pointSelected = Selected.Contains( point );
                        var primaryPointSelected = pointSelected && PrimarySelected == point;
                        var pointColor = primaryPointSelected ? Plugin.Configuration.CurveEditorPrimarySelectedColor : ( pointSelected ? Plugin.Configuration.CurveEditorSelectedColor : Plugin.Configuration.CurveEditorPointColor );
                        var pointSize = primaryPointSelected ? Plugin.Configuration.CurveEditorPrimarySelectedSize : ( pointSelected ? Plugin.Configuration.CurveEditorSelectedSize : Plugin.Configuration.CurveEditorPointSize );

                        if( IsColor ) ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels( point.GetImPlotPoint() ), pointSize + Plugin.Configuration.CurveEditorColorRingSize, Invert( point.RawData ) );

                        var tempX = point.DisplayX;
                        var tempY = point.DisplayY;
                        if( ImPlot.DragPoint( idx, ref tempX, ref tempY, IsColor ? new Vector4( point.RawData, 1 ) : pointColor, pointSize, ImPlotDragToolFlags.Delayed ) ) {
                            if( !IsPointDragged ) {
                                IsPointDragged = true;
                                PrePointDragState = GetState();
                            }
                            PointDragStartTime = DateTime.Now;

                            if( !pointSelected ) {
                                Selected.Clear();
                                Selected.Add( point );
                            }

                            // Dragging point around
                            var diffX = tempX - point.DisplayX;
                            var diffY = tempY - point.DisplayY;
                            foreach( var selected in Selected ) {
                                selected.DisplayX += diffX;
                                selected.DisplayY += diffY;
                            }

                            draggingAnyPoint = true;
                        }

                        if( idx > 0 && point.DisplayX < Points[idx - 1].DisplayX ) wrongOrder = true;
                        idx++;
                    }

                    if( IsPointDragged && !draggingAnyPoint && ( DateTime.Now - PointDragStartTime ).TotalMilliseconds > 200 ) {
                        IsPointDragged = false;
                        CommandManager.Avfx.Add( new UiCurveEditorDragCommand( this, PrePointDragState, GetState() ) );
                    }

                    // Selecting point [Left Click]
                    if( !draggingAnyPoint && ImGui.IsMouseDown( ImGuiMouseButton.Left ) && !ImGui.GetIO().KeyCtrl && IsHovering() ) {
                        var mousePos = ImGui.GetMousePos();
                        foreach( var point in Points ) {
                            if( ( ImPlot.PlotToPixels( point.GetImPlotPoint() ) - mousePos ).Length() < Plugin.Configuration.CurveEditorGrabbingDistance ) {
                                if( !ImGui.GetIO().KeyShift ) Selected.Clear();
                                if( !Selected.Contains( point ) ) Selected.Add( point );
                                break;
                            }
                        }
                    }
                }

                // Inserting point [Ctrl + Left Click]
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.GetIO().KeyCtrl && IsHovering() ) {
                    var pos = ImPlot.GetPlotMousePos();
                    var z = IsColor ? 1.0f : ( float )pos.y;
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => {
                        InsertPoint( ( float )pos.x, 1, 1, z );
                    } ) );
                    UpdateGradient();
                }

                ImPlot.EndPlot();
            }

            if( wrongOrder ) {
                ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "POINTS ARE IN THE WRONG ORDER" );
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"Sort{parentId}", true ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => { // Sort
                        Keys.Sort( ( x, y ) => x.Time.CompareTo( y.Time ) );
                        Points.Sort( ( x, y ) => x.DisplayX.CompareTo( y.DisplayX ) );
                    } ) );
                    UpdateGradient();
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            PrimarySelected?.Draw();
        }

        public void UpdateGradient() {
            if( IsColor && Keys.Count > 1 ) {
                var allSameTime = true;
                foreach( var key in Keys ) {
                    if( key.Time != Keys[0].Time ) {
                        allSameTime = false;
                        break;
                    }
                }
                if( allSameTime ) Points[^1].DisplayX++;
                Plugin.DirectXManager.GradientView.SetGradient( Curve );
            }
        }

        public UiCurveEditorState GetState() {
            return new UiCurveEditorState {
                Points = Keys.Select( key => new UiCurveEditorPointState {
                    Type = key.Type,
                    Time = key.Time,
                    X = key.X,
                    Y = key.Y,
                    Z = key.Z
                } ).ToArray()
            };
        }

        public void SetState( UiCurveEditorState state ) {
            Selected.Clear();
            Keys.Clear();
            Points.Clear();
            foreach( var point in state.Points ) {
                var newKey = new AvfxCurveKey( point.Type, point.Time, point.X, point.Y, point.Z );
                Keys.Add( newKey );
                Points.Add( new UiCurveEditorPoint( this, newKey, Type ) );
            }
            UpdateGradient();
        }

        private void InsertPoint( float time, float x, float y, float z ) {
            var insertIdx = 0;
            foreach( var p in Points ) {
                if( p.DisplayX > Math.Round( time ) ) break;
                insertIdx++;
            }
            var newKey = new AvfxCurveKey( KeyType.Linear, ( int ) Math.Round( time ), x, y, z );
            Keys.Insert( insertIdx, newKey );
            Points.Insert( insertIdx, new UiCurveEditorPoint( this, newKey, Type ) );
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

                if( p1.KeyType == KeyType.Linear || color ) {
                    // p1 should already be added
                    var pos = p2.GetPosition();
                    xs.Add( pos.X );
                    ys.Add( pos.Y );
                }
                else if( p1.KeyType == KeyType.Step ) {
                    // p1 should already be added
                    xs.Add( ( float )p2.DisplayX );
                    ys.Add( ( float )p1.DisplayY );

                    var pos = p2.GetPosition();
                    xs.Add( pos.X );
                    ys.Add( pos.Y );
                }
                else if( p1.KeyType == KeyType.Spline ) {
                    var p1_ = p1.GetPosition();
                    var p2_ = p2.GetPosition();
                    var midX = ( p2_.X - p1_.X ) / 2;
                    var handle1 = new Vector2( p1_.X + p1.RawData.X * midX, p1_.Y );
                    var handle2 = new Vector2( p2_.X - p1.RawData.Y * midX, p2_.Y );
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
