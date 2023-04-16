using Dalamud.Logging;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditor : IUiItem {
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

        private bool PrevClickState = false;
        private DateTime PrevClickTime = DateTime.Now;

        public UiCurveEditor( AvfxCurve curve, CurveType type ) {
            Curve = curve;
            Type = type;
            EditorId = EditorCount++;
        }

        public void Initialize() { // Keys finished reading
            foreach( var key in Keys ) Points.Add( new UiCurveEditorPoint( this, key, Type ) );
        }

        public void Draw( string parentId ) {
            ImPlot.PushStyleVar( ImPlotStyleVar.FitPadding, new Vector2( 1f, 0.5f ) );

            DrawControls( parentId );
            Selected.RemoveAll( x => !Points.Contains( x ) );

            var wrongOrder = false;
            if( IsColor ) ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );

            if( ImPlot.BeginPlot( $"{parentId}-Plot-{EditorId}", new Vector2( -1, 300 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, IsColor ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );
                var clickState = IsHovering() && ImGui.IsMouseDown( ImGuiMouseButton.Left );

                if( Points.Count > 0 ) {
                    GetDrawLine( Points, IsColor, out var lineXs, out var lineYs );
                    var xs = lineXs.ToArray();
                    var ys = lineYs.ToArray();

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

                        // Dragging point
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
                    // want to ignore if going to drag points around, so only process if click+release is less than 200 ms
                    var processClick = !clickState && PrevClickState && ( DateTime.Now - PrevClickTime ).TotalMilliseconds < 200;
                    if( !draggingAnyPoint && processClick && !ImGui.GetIO().KeyCtrl && IsHovering() ) {
                        var mousePos = ImGui.GetMousePos();
                        foreach( var point in Points ) {
                            if( ( ImPlot.PlotToPixels( point.GetImPlotPoint() ) - mousePos ).Length() < Plugin.Configuration.CurveEditorGrabbingDistance ) {
                                if( !ImGui.GetIO().KeyShift ) Selected.Clear();
                                if( !Selected.Contains( point ) ) Selected.Add( point );
                                break;
                            }
                        }
                    }

                    // Box selection [Right-click, drag, then left-click]
                    if( ImPlot.IsPlotSelected() ) {
                        var selection = ImPlot.GetPlotSelection();
                        if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) {
                            ImPlot.CancelPlotSelection();

                            Selected.Clear();
                            foreach( var point in Points ) {
                                var pos = point.GetImPlotPoint();
                                if( pos.x <= selection.X.Max && pos.x >= selection.X.Min && pos.y <= selection.Y.Max && pos.y >= selection.Y.Min ) Selected.Add( point );
                            }
                        }
                    }
                }

                // Inserting point [Ctrl + Left Click]
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.GetIO().KeyCtrl && IsHovering() ) {
                    var pos = ImPlot.GetPlotMousePos();
                    var z = IsColor ? 1.0f : ( float )ToRadians( pos.y );
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( this, () => {
                        InsertPoint( ( float )pos.x, 1, 1, z );
                    } ) );
                    UpdateGradient();
                }

                if( clickState && !PrevClickState ) {
                    PrevClickTime = DateTime.Now;
                }
                PrevClickState = clickState;

                ImPlot.EndPlot();
            }

            ImPlot.PopStyleVar( 1 );

            if( wrongOrder ) DrawWrongOrder( parentId );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            PrimarySelected?.Draw();
        }

        private void DrawControls( string parentId ) {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            ImGui.TextDisabled( "Curve editor controls (?)" );
            UiUtils.Tooltip( "Ctrl + left-click to add a new point\n" +
                "Left-click to select a point, hold shift to select multiple\n" +
                "Right-click, drag, then left-click to perform a box selection" );

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
        }

        private void DrawWrongOrder( string parentId ) {
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

        public double ToRadians( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( Math.PI / 180 ) * value;
        }

        public double ToDegrees( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( 180 / Math.PI ) * value;
        }

        private void InsertPoint( float time, float x, float y, float z ) {
            var insertIdx = 0;
            foreach( var p in Points ) {
                if( p.DisplayX > Math.Round( time ) ) break;
                insertIdx++;
            }
            var newKey = new AvfxCurveKey( KeyType.Linear, ( int )Math.Round( time ), x, y, z );
            Keys.Insert( insertIdx, newKey );
            Points.Insert( insertIdx, new UiCurveEditorPoint( this, newKey, Type ) );
        }

        // ==========================

        private static void GetDrawLine( List<UiCurveEditorPoint> points, bool color, out List<double> xs, out List<double> ys ) {
            xs = new List<double>();
            ys = new List<double>();

            if( points.Count > 0 ) {
                xs.Add( points[0].DisplayX );
                ys.Add( points[0].DisplayY );
            }

            for( var idx = 1; idx < points.Count; idx++ ) {
                var p1 = points[idx - 1];
                var p2 = points[idx];

                if( p1.KeyType == KeyType.Linear || color ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.KeyType == KeyType.Step ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p1.DisplayY );

                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.KeyType == KeyType.Spline ) {
                    var p1X = p1.DisplayX;
                    var p1Y = p1.DisplayY;
                    var p2X = p2.DisplayX;
                    var p2Y = p2.DisplayY;

                    var midX = ( p2X - p1X ) / 2;

                    var handle1X = p1X + p1.RawData.X * midX;
                    var handle1Y = p1Y;
                    var handle2X = p2X - p1.RawData.Y * midX;
                    var handle2Y = p2Y;

                    for( var i = 0; i < 100; i++ ) {
                        var t = i / 99.0d;

                        Bezier( p1X, p1Y, p2X, p2Y, handle1X, handle1Y, handle2X, handle2Y, t, out var bezX, out var bezY );
                        xs.Add( bezX );
                        ys.Add( bezY );
                    }
                    xs.Add( p2X );
                    ys.Add( p2Y );
                }
            }
        }

        /*
         * X and Y are multiplied by 15, then converted to int, and then stored in a signed byte, so can only be -128 to 128
         * need more research into how they actually work, and what impact "linear" vs "spline" actually has
         * 
         * E8 ? ? ? ? EB 3C 8B 46 04
         * ffxiv_dx11.exe.text+402BE7
         * readFloatLocal(debug_getXMMPointer(1)) == 60.0
         * 
         *  v5[2 * i + 1] = Z;
            LOWORD(v5[2 * i + 2]) ^= (Time ^ LOWORD(v5[2 * i + 2])) & 0x3FFF;
            LOWORD(v5[2 * i + 2]) = v5[2 * i + 2] & 0x3FFF | (Type << 14);
            BYTE2(v5[2 * i + 2]) = (X * 15.0);
            HIBYTE(v5[2 * i + 2]) = (Y * 15.0);

            // Time (0)
            // Type (2)
            // X (4)
            // Y (8)
            // Z (12)

            // Spline = 0
            // Linear = 1
            // Step = 2
         */

        private static bool IsHovering() {
            var mousePos = ImGui.GetMousePos();
            var topLeft = ImPlot.GetPlotPos();
            var plotSize = ImPlot.GetPlotSize();
            if( mousePos.X >= topLeft.X && mousePos.X < topLeft.X + plotSize.X && mousePos.Y >= topLeft.Y && mousePos.Y < topLeft.Y + plotSize.Y ) return true;
            return false;
        }

        private static uint Invert( Vector3 color ) => color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32( new Vector4( 0, 0, 0, 1 ) ) : ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );

        private static void Bezier( double p1X, double p1Y, double p2X, double p2Y, double handle1X, double handle1Y, double handle2X, double handle2Y, double t, out double x, out double y ) {
            var u = 1 - t;
            var w1 = u * u * u;
            var w2 = 3 * u * u * t;
            var w3 = 3 * u * t * t;
            var w4 = t * t * t;

            x = w1 * p1X + w2 * handle1X + w3 * handle2X + w4 * p2X;
            y = w1 * p1Y + w2 * handle1Y + w3 * handle2Y + w4 * p2Y;
        }
    }
}
