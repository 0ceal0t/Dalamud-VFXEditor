using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxCurve {
        private static readonly List<(KeyType, Vector4)> CopiedKeys = [];

        private readonly List<AvfxCurveKey> Selected = [];
        private AvfxCurveKey SelectedPrimary => Selected.Count == 0 ? null : Selected[0];

        private bool DrawOnce = false;
        public bool IsColor => Type == CurveType.Color;

        private bool PrevClickState = false;
        private DateTime PrevClickTime = DateTime.Now;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;

        private float PopupFrame;

        public void RemoveKey( AvfxCurveKey key ) {
            Keys.Remove( key );
            Selected.Remove( key );
        }

        private void DrawEditor() {
            using var _ = ImRaii.PushId( Id );

            DrawControls( out var fit );
            Selected.RemoveAll( x => !Keys.Contains( x ) );

            if( !DrawOnce ) {
                fit = true;
                DrawOnce = true;
            }

            var wrongOrder = false;
            var height = ImGui.GetContentRegionAvail().Y - ( 4 * ImGui.GetFrameHeightWithSpacing() + 5 );

            ImPlot.PushStyleVar( ImPlotStyleVar.FitPadding, new Vector2( 0.5f, 0.5f ) );
            if( ImPlot.BeginPlot( "##CurveEditor", new Vector2( -1, height ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                if( fit ) ImPlot.SetNextAxesToFit();
                if( IsColor ) {
                    ImPlot.SetupAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
                    ImPlot.SetupAxisLimitsConstraints( ImAxis.X1, 0, double.MaxValue - 1 );
                }

                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, IsColor ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );
                var clickState = IsHovering() && ImGui.IsMouseDown( ImGuiMouseButton.Left );

                if( Keys.Count > 0 ) {
                    GetDrawLine( Keys, IsColor, out var lineXs, out var lineYs );
                    var xs = lineXs.ToArray();
                    var ys = lineYs.ToArray();

                    ImPlot.SetNextLineStyle( Plugin.Configuration.CurveEditorLineColor, Plugin.Configuration.CurveEditorLineWidth );
                    ImPlot.PlotLine( AvfxName, ref xs[0], ref ys[0], xs.Length );

                    DrawGradient();
                    var draggingAnyPoint = false;
                    foreach( var (key, idx) in Keys.WithIndex() ) {
                        var isSelected = Selected.Contains( key );
                        var isPimary = isSelected && SelectedPrimary == key;
                        var pointColor = isPimary ? Plugin.Configuration.CurveEditorPrimarySelectedColor : ( isSelected ? Plugin.Configuration.CurveEditorSelectedColor : Plugin.Configuration.CurveEditorPointColor );
                        var pointSize = isPimary ? Plugin.Configuration.CurveEditorPrimarySelectedSize : ( isSelected ? Plugin.Configuration.CurveEditorSelectedSize : Plugin.Configuration.CurveEditorPointSize );

                        if( IsColor ) ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels( key.Point ), pointSize + Plugin.Configuration.CurveEditorColorRingSize, Invert( key.Color ) );

                        var x = key.DisplayX;
                        var y = key.DisplayY;

                        // Dragging point
                        if( ImPlot.DragPoint( idx, ref x, ref y, IsColor ? new Vector4( key.Color, 1 ) : pointColor, pointSize, ImPlotDragToolFlags.Delayed ) ) {
                            if( !isSelected ) {
                                Selected.Clear();
                                Selected.Add( key );
                            }

                            if( !Editing ) {
                                Editing = true;
                                Selected.ForEach( x => x.StartDragging() );
                            }
                            LastEditTime = DateTime.Now;

                            var diffX = x - key.DisplayX;
                            var diffY = y - key.DisplayY;
                            foreach( var selected in Selected ) {
                                selected.DisplayX += diffX;
                                selected.DisplayY += diffY;
                            }

                            draggingAnyPoint = true;
                        }

                        if( idx > 0 && key.DisplayX < Keys[idx - 1].DisplayX ) wrongOrder = true;
                    }

                    if( Editing && !draggingAnyPoint && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                        Editing = false;
                        var commands = new List<ICommand>();
                        Selected.ForEach( x => x.StopDragging( commands ) );
                        CommandManager.Add( new CompoundCommand( commands, Update ) );
                    }

                    // Selecting point [Left Click]
                    // want to ignore if going to drag points around, so only process if click+release is less than 200 ms
                    var processClick = !clickState && PrevClickState && ( DateTime.Now - PrevClickTime ).TotalMilliseconds < 200;
                    if( !draggingAnyPoint && processClick && !ImGui.GetIO().KeyCtrl && IsHovering() && !ImGui.IsAnyItemActive() && ImGui.IsWindowFocused() ) {
                        SingleSelect();
                    }

                    // Box selection [Right-click, drag, then left-click]
                    if( ImPlot.IsPlotSelected() ) BoxSelect();
                    else {
                        // Popup [Right-click]
                        if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) {
                            ImGui.OpenPopup( "CurvePopup" );
                            PopupFrame = ( float )Math.Round( ImPlot.GetPlotMousePos().x );
                        }

                        using var popup = ImRaii.Popup( "CurvePopup" );
                        if( popup ) {
                            using var disabled = ImRaii.Disabled( CopiedKeys.Count == 0 );
                            if( ImGui.Selectable( $"Paste at frame {( int )PopupFrame}" ) ) PasteAtFrame( PopupFrame );
                        }
                    }
                }

                // Inserting point [Ctrl + Left Click]
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.GetIO().KeyCtrl && IsHovering() && ImGui.IsWindowFocused() ) NewPoint();

                if( clickState && !PrevClickState ) {
                    PrevClickTime = DateTime.Now;
                }
                PrevClickState = clickState;

                ImPlot.EndPlot();
            }

            ImPlot.PopStyleVar( 1 );

            if( wrongOrder ) DrawWrongOrder();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            SelectedPrimary?.Draw();
        }

        private void DrawControls( out bool fit ) {
            fit = false;
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Type == CurveType.Angle ) {
                if( ImGui.RadioButton( "Radians", !Plugin.Configuration.UseDegreesForAngles ) ) {
                    Plugin.Configuration.UseDegreesForAngles = false;
                    Plugin.Configuration.Save();
                }
                ImGui.SameLine();
                if( ImGui.RadioButton( "Degrees", Plugin.Configuration.UseDegreesForAngles ) ) {
                    Plugin.Configuration.UseDegreesForAngles = true;
                    Plugin.Configuration.Save();
                }
            }

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.SmallButton( "Fit" ) ) fit = true;

                ImGui.SameLine();
                if( UiUtils.DisabledButton( "Copy", Keys.Count > 0, true ) ) Copy();

                ImGui.SameLine();
                if( UiUtils.DisabledButton( "Paste", CopiedKeys.Count > 0, true ) ) Paste();

                ImGui.SameLine();
                if( UiUtils.RemoveButton( "Clear", true ) ) Clear();
            }

            ImGui.SameLine();
            UiUtils.IconText( FontAwesomeIcon.InfoCircle, true );
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();

                var color = UiUtils.PARSED_GREEN;
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemInnerSpacing.X, ImGui.GetStyle().ItemSpacing.Y ) );

                ImGui.TextColored( color, "Ctrl+Left Click" );
                ImGui.SameLine();
                ImGui.Text( "to add a new point" );

                ImGui.TextColored( color, "Left Click" );
                ImGui.SameLine();
                ImGui.Text( "to selected a point" );

                ImGui.Text( "Hold" );
                ImGui.SameLine();
                ImGui.TextColored( color, "Shift" );
                ImGui.SameLine();
                ImGui.Text( "to select multiple points" );

                ImGui.EndTooltip();
            }
        }

        private void DrawWrongOrder() {
            ImGui.TextColored( UiUtils.RED_COLOR, "POINTS ARE IN THE WRONG ORDER" );
            ImGui.SameLine();
            if( UiUtils.RemoveButton( "Sort", true ) ) {
                var sorted = new List<AvfxCurveKey>( Keys );
                sorted.Sort( ( x, y ) => x.Time.Value.CompareTo( y.Time.Value ) );
                CommandManager.Add( new ListSetCommand<AvfxCurveKey>( Keys, sorted, Update ) );
            }
        }

        public double ToRadians( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( Math.PI / 180 ) * value;
        }

        public double ToDegrees( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value;
            return ( 180 / Math.PI ) * value;
        }

        // ========== FUNCTIONS ============

        private void SingleSelect() {
            var mousePos = ImGui.GetMousePos();
            foreach( var key in Keys ) {
                if( ( ImPlot.PlotToPixels( key.Point ) - mousePos ).Length() < Plugin.Configuration.CurveEditorGrabbingDistance ) {
                    if( !ImGui.GetIO().KeyShift ) Selected.Clear();
                    if( !Selected.Contains( key ) ) Selected.Add( key );
                    break;
                }
            }
        }

        private void BoxSelect() {
            var selection = ImPlot.GetPlotSelection();
            if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) {
                ImPlot.CancelPlotSelection();
                Selected.Clear();
                foreach( var key in Keys ) {
                    var point = key.Point;
                    if( point.x <= selection.X.Max && point.x >= selection.X.Min && point.y <= selection.Y.Max && point.y >= selection.Y.Min ) Selected.Add( key );
                }
            }
        }

        private void NewPoint() {
            var pos = ImPlot.GetPlotMousePos();
            var time = Math.Round( pos.x );
            var insertIdx = 0;
            foreach( var key in Keys ) {
                if( key.DisplayX > time ) break;
                insertIdx++;
            }

            CommandManager.Add( new ListAddCommand<AvfxCurveKey>(
                Keys,
                new AvfxCurveKey( this, KeyType.Linear, ( int )time, 1, 1, IsColor ? 1.0f : ( float )ToRadians( pos.y ) ),
                insertIdx,
                ( AvfxCurveKey _, bool _ ) => Update()
            ) );
        }

        private void Copy() {
            CopiedKeys.Clear();
            foreach( var key in Keys ) CopiedKeys.Add( key.CopyPasteData );
        }

        private void Paste() {
            var commands = new List<ICommand>();
            foreach( var key in CopiedKeys ) commands.Add( new ListAddCommand<AvfxCurveKey>( Keys, new( this, key ) ) );
            CommandManager.Add( new CompoundCommand( commands, Update ) );
        }

        private void PasteAtFrame( float frame ) {
            var firstFrame = CopiedKeys[0].Item2.X;
            var commands = new List<ICommand>();
            foreach( var key in CopiedKeys ) commands.Add( new ListAddCommand<AvfxCurveKey>( Keys,
                    new( this, (key.Item1, key.Item2 with { X = key.Item2.X - firstFrame + frame }) )
                ) );
            CommandManager.Add( new CompoundCommand( commands, Update ) );
        }

        private void Clear() {
            CommandManager.Add( new ListSetCommand<AvfxCurveKey>( Keys, [], Update ) );
        }

        // ======== GRADIENT ==========

        public void Update() {
            KeyList.SetAssigned( true );

            if( !IsColor || Keys.Count < 2 ) return;
            if( !Keys.FindFirst( x => x.Time.Value != Keys[0].Time.Value, out var _ ) ) { // All have the same time
                Keys[^1].Time.Value++;
            }

            UpdateGradient();
        }

        private void DrawGradient() {
            if( !IsColor || Keys.Count < 2 ) return;
            if( Plugin.DirectXManager.GradientView.CurrentRenderId != RenderId ) UpdateGradient();

            var topLeft = new ImPlotPoint { x = Keys[0].DisplayX, y = 1 };
            var bottomRight = new ImPlotPoint { x = Keys[^1].DisplayX, y = -1 };
            ImPlot.PlotImage( "##Gradient", Plugin.DirectXManager.GradientView.Output, topLeft, bottomRight );
        }

        private void UpdateGradient() {
            Plugin.DirectXManager.GradientView.SetGradient( RenderId, [
                Keys.Select( x => (x.Time.Value, x.Color)).ToList()
            ] );
        }

        // ======== UTILS ===========

        private static void GetDrawLine( List<AvfxCurveKey> points, bool color, out List<double> xs, out List<double> ys ) {
            xs = [];
            ys = [];

            if( points.Count > 0 ) {
                xs.Add( points[0].DisplayX );
                ys.Add( points[0].DisplayY );
            }

            for( var idx = 1; idx < points.Count; idx++ ) {
                var p1 = points[idx - 1];
                var p2 = points[idx];

                if( p1.Type.Value == KeyType.Linear || color ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.Type.Value == KeyType.Step ) {
                    // p1 should already be added
                    xs.Add( p2.DisplayX );
                    ys.Add( p1.DisplayY );

                    xs.Add( p2.DisplayX );
                    ys.Add( p2.DisplayY );
                }
                else if( p1.Type.Value == KeyType.Spline ) {
                    var p1X = p1.DisplayX;
                    var p1Y = p1.DisplayY;
                    var p2X = p2.DisplayX;
                    var p2Y = p2.DisplayY;

                    var midX = ( p2X - p1X ) / 2;

                    var handle1X = p1X + p1.Converted.X * midX;
                    var handle1Y = p1Y;
                    var handle2X = p2X - p1.Converted.Y * midX;
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
