using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.AvfxFormat;
using VfxEditor.DirectX;
using VfxEditor.Formats.AvfxFormat.Assign;
using VfxEditor.Parsing;
using VfxEditor.Utils;
using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.Formats.AvfxFormat.Curve.Lines {
    public enum LinesAssigned {
        None,
        Some,
        All
    }

    public class LineEditorGroup {
        public readonly string Name;
        public readonly List<AvfxCurveData> Curves;
        public IEnumerable<AvfxCurveData> AssignedCurves => Curves.Where( x => x.IsAssigned() );
        public readonly AvfxDrawable? ConnectType;

        public readonly int RenderId = Renderer.NewId;

        private static readonly CurveBehavior[] CurveBehaviorOptions = ( CurveBehavior[] )Enum.GetValues( typeof( CurveBehavior ) );
        private static readonly RandomType[] RandomTypeOptions = ( RandomType[] )Enum.GetValues( typeof( RandomType ) );
        private static readonly Vector4[] LineColors = [ // TODO
            new( 1, 1, 0, 1),
            new( 0, 1, 1, 1),
            new( 1, 0, 1, 1),
        ];

        public LinesAssigned Assigned {
            get {
                var numAssigned = AssignedCurves.Count();
                if( numAssigned == 0 ) return LinesAssigned.None;
                else if( numAssigned == Curves.Count ) return LinesAssigned.All;
                return LinesAssigned.Some;
            }
        }

        private bool DrawOnce = false;
        private bool IsColor => Curves.Count == 1 && Curves[0].IsColor;
        private AvfxCurveData? ColorCurve => IsColor ? Curves[0] : null;

        private readonly List<(AvfxCurveData, AvfxCurveKey)> Selected = [];
        private (AvfxCurveData?, AvfxCurveKey?) SelectedPrimary => Selected.Count == 0 ? (null, null) : Selected[0];

        private AvfxCurveData? PrimaryCurve => SelectedPrimary.Item1;
        private AvfxCurveKey? PrimaryKey => SelectedPrimary.Item2;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;

        public LineEditorGroup( AvfxCurveData curve ) {
            Name = curve.Name;
            Curves = [curve];
        }

        public LineEditorGroup( string name, List<AvfxCurveData> curves, AvfxDrawable? connectType ) {
            Name = name;
            Curves = curves;
            ConnectType = connectType;

        }

        public void Draw() {
            ConnectType?.Draw();
            DrawTable();
            DrawEditor();
        }

        public void DrawTable() {
            var height = ( ImGui.GetFrameHeightWithSpacing() + 3 ) * ( Curves.Count + 1 );

            using var _ = ImRaii.PushId( "##Table" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 4 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", new( -1, height ), false );
            using var table = ImRaii.Table( "Table", 5,
                ImGuiTableFlags.RowBg | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.PadOuterX );
            if( !table ) return;

            padding.Dispose();

            ImGui.TableSetupScrollFreeze( 0, 1 );

            ImGui.TableSetupColumn( "##Check", ImGuiTableColumnFlags.None, -1 );
            ImGui.TableSetupColumn( "##Name", ImGuiTableColumnFlags.None, -1 );

            ImGui.TableSetupColumn( "Pre Behavior", ImGuiTableColumnFlags.WidthStretch, -1 );
            ImGui.TableSetupColumn( "Post Behavior", ImGuiTableColumnFlags.WidthStretch, -1 );
            ImGui.TableSetupColumn( "Random Type", ImGuiTableColumnFlags.WidthStretch, -1 );

            ImGui.TableHeadersRow();

            foreach( var (curve, idx) in Curves.WithIndex() ) {
                ImGui.TableNextRow();
                using var __ = ImRaii.PushId( idx );

                ImGui.TableNextColumn();
                var assigned = curve.IsAssigned();
                using( var locked = ImRaii.Disabled( curve.Locked && curve.IsAssigned() ) ) {
                    if( ImGui.Checkbox( "##Assigned", ref assigned ) ) CommandManager.Add( new AvfxAssignCommand( curve, assigned, recurse: true ) );
                }

                using var disabled = ImRaii.Disabled( !curve.IsAssigned() );

                ImGui.TableNextColumn();
                ImGui.Text( curve.Name );

                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X );
                if( UiUtils.EnumComboBox( "##Pre", CurveBehaviorOptions, curve.PreBehavior.Value, out var newPre ) ) {
                    CommandManager.Add( new ParsedSimpleCommand<CurveBehavior>( curve.PreBehavior.Parsed, newPre ) );
                }

                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X );
                if( UiUtils.EnumComboBox( "##Post", CurveBehaviorOptions, curve.PostBehavior.Value, out var newPost ) ) {
                    CommandManager.Add( new ParsedSimpleCommand<CurveBehavior>( curve.PostBehavior.Parsed, newPost ) );
                }

                ImGui.TableNextColumn();
                if( curve.Type != CurveType.Color ) {
                    ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X );
                    if( UiUtils.EnumComboBox( "##Random", RandomTypeOptions, curve.Random.Value, out var newRandom ) ) {
                        CommandManager.Add( new ParsedSimpleCommand<RandomType>( curve.Random.Parsed, newRandom ) );
                    }
                }
            }
        }

        public unsafe void DrawEditor() {
            // TODO: copy paste, try to match up names

            using var _ = ImRaii.PushId( "##Lines" );

            var fit = false;
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

                var draggingAnyPoint = false;
                var dragPointId = 0;
                foreach( var (curve, curveIdx) in Curves.WithIndex() ) {
                    if( !curve.IsAssigned() || curve.Keys.Count == 0 ) continue;

                    ImPlot.HideNextItem( false, ImPlotCond.Once );
                    curve.GetDrawLine( out var _xs, out var _ys );
                    var xs = _xs.ToArray();
                    var ys = _ys.ToArray();

                    var lineColor = curve.Name switch {
                        "X" or "RX" => new( 1, 0, 0, 1 ),
                        "Y" or "RY" => new( 0, 1, 0, 1 ),
                        "Z" or "RZ" => new( 0, 0, 1, 1 ),
                        _ => LineColors[curveIdx % LineColors.Length],
                    };

                    ImPlot.SetNextLineStyle( lineColor, Plugin.Configuration.CurveEditorLineWidth );

                    ImPlot.PlotLine( curve.Name, ref xs[0], ref ys[0], xs.Length );

                    DrawGradient();

                    foreach( var (key, keyIdx) in curve.Keys.WithIndex() ) {
                        dragPointId++;

                        var isSelected = Selected.Any( x => x.Item2 == key );
                        var isPrimarySelected = PrimaryKey == key;

                        var pointSize = isPrimarySelected ? Plugin.Configuration.CurveEditorPrimarySelectedSize : ( isSelected ? Plugin.Configuration.CurveEditorSelectedSize : Plugin.Configuration.CurveEditorPointSize );
                        if( IsColor ) ImPlot.GetPlotDrawList().AddCircleFilled( ImPlot.PlotToPixels( key.Point ), pointSize + Plugin.Configuration.CurveEditorColorRingSize, Invert( key.Color ) );

                        var x = key.DisplayX;
                        var y = key.DisplayY;

                        // Dragging point
                        if( ImPlot.DragPoint( dragPointId, ref x, ref y, IsColor ? new Vector4( key.Color, 1 ) : lineColor, pointSize ) ) {
                            if( !isSelected ) {
                                Selected.Clear();
                                Selected.Add( (curve, key) );
                            }

                            if( !Editing ) {
                                Editing = true;
                                Selected.ForEach( x => x.Item2.StartDragging() );
                            }
                            LastEditTime = DateTime.Now;

                            var diffX = x - key.DisplayX;
                            var diffY = y - key.DisplayY;
                            foreach( var selected in Selected ) {
                                selected.Item2.DisplayX += diffX;
                                selected.Item2.DisplayY += diffY;
                            }

                            draggingAnyPoint = true;
                        }

                        if( keyIdx > 0 && key.DisplayX < curve.Keys[keyIdx - 1].DisplayX ) wrongOrder = true;
                    }

                    // ======================

                    if( Editing && !draggingAnyPoint && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                        Editing = false;
                        var commands = new List<ICommand>();
                        Selected.ForEach( x => x.Item2.StopDragging( commands ) );
                        CommandManager.Add( new CompoundCommand( commands, OnUpdate ) );
                    }

                    // TODO: box select and right click
                }

                // Inserting point [Ctrl + Left Click]
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.GetIO().KeyCtrl && IsHovering() && ImGui.IsWindowFocused() ) {
                    // TODO
                    // new point
                }

                ImPlot.EndPlot();

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                PrimaryKey?.Draw();
            }

            ImPlot.PopStyleVar( 1 );

            if( wrongOrder ) {
                ImGui.TextColored( UiUtils.RED_COLOR, "POINTS ARE IN THE WRONG ORDER" );
                ImGui.SameLine();
                if( UiUtils.RemoveButton( "Sort", true ) ) {
                    var commands = new List<ICommand>();
                    foreach( var curve in AssignedCurves ) curve.Sort( commands );
                    CommandManager.Add( new CompoundCommand( commands, UpdateGradient ) );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            PrimaryKey?.Draw();
        }

        public void OnUpdate() {
            foreach( var curve in Curves.Where( x => x.IsAssigned() ) ) curve.OnUpdate();
            UpdateGradient();
        }

        public void DrawGradient() {
            if( !IsColor || ColorCurve!.Keys.Count < 2 ) return;
            if( Plugin.DirectXManager.GradientView.CurrentRenderId != RenderId ) UpdateGradient();

            var topLeft = new ImPlotPoint { x = ColorCurve.Keys[0].DisplayX, y = 1 };
            var bottomRight = new ImPlotPoint { x = ColorCurve.Keys[^1].DisplayX, y = -1 };
            ImPlot.PlotImage( "##Gradient", Plugin.DirectXManager.GradientView.Output, topLeft, bottomRight );
        }

        private void UpdateGradient() {
            if( !IsColor || ColorCurve!.Keys.Count < 2 ) return;
            Plugin.DirectXManager.GradientView.SetGradient( RenderId, [
                ColorCurve.Keys.Select( x => (x.Time.Value, x.Color)).ToList()
            ] );
        }

        // TODO: allow hiding in legend
        private void SingleSelect() {
            var mousePos = ImGui.GetMousePos();
            foreach( var curve in Curves.Where( x => x.IsAssigned() ) ) {
                foreach( var key in curve.Keys ) {
                    if( ( ImPlot.PlotToPixels( key.Point ) - mousePos ).Length() < Plugin.Configuration.CurveEditorGrabbingDistance ) {
                        if( !ImGui.GetIO().KeyShift ) Selected.Clear();
                        if( !Selected.Any( x => x.Item2 == key ) ) Selected.Add( (curve, key) );
                        break;
                    }
                }
            }
        }

        private static uint Invert( Vector3 color ) => color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32( new Vector4( 0, 0, 0, 1 ) ) : ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );

        private static bool IsHovering() {
            var mousePos = ImGui.GetMousePos();
            var topLeft = ImPlot.GetPlotPos();
            var plotSize = ImPlot.GetPlotSize();
            if( mousePos.X >= topLeft.X && mousePos.X < topLeft.X + plotSize.X && mousePos.Y >= topLeft.Y && mousePos.Y < topLeft.Y + plotSize.Y ) return true;
            return false;
        }
    }
}
