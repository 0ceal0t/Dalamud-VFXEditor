using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.AvfxFormat;
using VfxEditor.Data.Command.ListCommands;
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

        private static readonly CurveBehavior[] CurveBehaviorOptions = Enum.GetValues<CurveBehavior>();
        private static readonly RandomType[] RandomTypeOptions = Enum.GetValues<RandomType>();
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

        private AvfxCurveKey? PrimaryKey => SelectedPrimary.Item2;

        private bool PrevClickState = false;
        private DateTime PrevClickTime = DateTime.Now;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;

        private ImPlotPoint SavedPoint = new();

        private static readonly Dictionary<string, List<(KeyType, Vector4)>> CopiedKeys = [];

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
            using( ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.Separator();
            }
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

        private void DrawControls( out bool fit ) {
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                fit = ImGui.Button( $"{FontAwesomeIcon.ExpandArrowsAlt.ToIconString()}" );

                ImGui.SameLine();
                if( ImGui.Button( $"{FontAwesomeIcon.Copy.ToIconString()}" ) ) Copy();

                ImGui.SameLine();
                if( UiUtils.DisabledButton( $"{FontAwesomeIcon.Paste.ToIconString()}", CopiedKeys.Count > 0 ) ) Paste();

                ImGui.SameLine();
                if( UiUtils.DisabledButton( $"{FontAwesomeIcon.ArrowRightToBracket.ToIconString()}", CopiedKeys.Count > 0 ) ) Replace();

                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{FontAwesomeIcon.Times.ToIconString()}" ) ) Clear();
            }

            if( Curves.Any( x => x.Type == CurveType.Angle ) ) {
                ImGui.SameLine();
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

            foreach( var curve in AssignedCurves ) {
                foreach( var (key, idx) in curve.Keys.WithIndex() ) {
                    if( idx > 0 && key.DisplayX < curve.Keys[idx - 1].DisplayX ) {
                        // Two points in the wrong order
                        ImGui.SameLine();
                        if( UiUtils.RemoveButton( "Sort" ) ) {
                            var commands = new List<ICommand>();
                            foreach( var x in AssignedCurves ) x.Sort( commands );
                            CommandManager.Add( new CompoundCommand( commands, UpdateGradient ) );
                        }
                        return;
                    }
                }
            }
        }

        public unsafe void DrawEditor() {
            DrawControls( out var fit );

            using var _ = ImRaii.PushId( "##Lines" );

            Selected.RemoveAll( x => !Curves.Contains( x.Item1 ) || !x.Item1.Keys.Contains( x.Item2 ) );

            if( !DrawOnce ) {
                fit = true;
                DrawOnce = true;
            }

            var height = ImGui.GetContentRegionAvail().Y - ( 4 * ImGui.GetFrameHeightWithSpacing() + 5 );
            ImPlot.PushStyleVar( ImPlotStyleVar.FitPadding, new Vector2( 0.5f, 0.5f ) );
            if( ImPlot.BeginPlot( "##CurveEditor", new Vector2( -1, height ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                if( fit ) ImPlot.SetNextAxesToFit();
                if( IsColor ) {
                    ImPlot.SetupAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
                    ImPlot.SetupAxisLimitsConstraints( ImAxis.X1, 0, double.MaxValue - 1 );
                }

                ImPlot.SetupLegend( ImPlotLocation.NorthWest, ImPlotLegendFlags.NoButtons );
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, IsColor ? ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel : ImPlotAxisFlags.NoLabel );

                var clickState = IsHovering() && ImGui.IsMouseDown( ImGuiMouseButton.Left );

                var draggingAnyPoint = false;
                var dragPointId = 0;
                foreach( var (curve, curveIdx) in Curves.WithIndex() ) {
                    if( !curve.IsAssigned() || curve.Keys.Count == 0 ) continue;

                    ImPlot.HideNextItem( false, ImPlotCond.Once );
                    curve.GetDrawLine( out var _xs, out var _ys );
                    var xs = _xs.ToArray();
                    var ys = _ys.ToArray();

                    var lineColor = curve.GetAvfxName() switch {
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
                        if( ImPlot.DragPoint( dragPointId, ref x, ref y, IsColor ? new Vector4( key.Color, 1 ) : lineColor, pointSize, ImPlotDragToolFlags.Delayed ) ) {
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
                    }

                    // ======================

                    if( Editing && !draggingAnyPoint && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                        Editing = false;
                        var commands = new List<ICommand>();
                        Selected.ForEach( x => x.Item2.StopDragging( commands ) );
                        CommandManager.Add( new CompoundCommand( commands, OnUpdate ) );
                    }

                    // Selecting point [Left Click]
                    // want to ignore if going to drag points around, so only process if click+release is less than 200 ms
                    var processClick = !clickState && PrevClickState && ( DateTime.Now - PrevClickTime ).TotalMilliseconds < 200;
                    if( !draggingAnyPoint && processClick && !ImGui.GetIO().KeyCtrl && IsHovering() && !ImGui.IsAnyItemActive() && ImGui.IsWindowFocused() ) SingleSelect();

                    // TODO: box select and right click
                }

                // Inserting point [Ctrl + Left Click]
                if( ImGui.IsMouseClicked( ImGuiMouseButton.Left ) && ImGui.GetIO().KeyCtrl && IsHovering() && ImGui.IsWindowFocused() ) NewPoint();
                using( var popup = ImRaii.Popup( "NewPointPopup" ) ) {
                    if( popup ) {
                        foreach( var curve in AssignedCurves ) {
                            if( ImGui.Selectable( curve.GetText() ) ) NewPoint( curve, SavedPoint );
                        }
                    }
                }

                if( clickState && !PrevClickState ) PrevClickTime = DateTime.Now;
                PrevClickState = clickState;

                ImPlot.EndPlot();
            }

            ImPlot.PopStyleVar( 1 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            PrimaryKey?.Draw( this );
        }

        private void SingleSelect() {
            var mousePos = ImGui.GetMousePos();
            foreach( var curve in AssignedCurves ) {
                foreach( var key in curve.Keys ) {
                    if( ( ImPlot.PlotToPixels( key.Point ) - mousePos ).Length() < Plugin.Configuration.CurveEditorGrabbingDistance ) {
                        if( !ImGui.GetIO().KeyShift ) Selected.Clear();
                        if( !Selected.Contains( (curve, key) ) ) Selected.Add( (curve, key) );
                        return;
                    }
                }
            }

            if( !ImGui.GetIO().KeyShift ) Selected.Clear(); // nothing clicked, clear everything
        }

        private void NewPoint() {
            var point = ImPlot.GetPlotMousePos();
            if( AssignedCurves.Count() == 1 ) NewPoint( AssignedCurves.First(), point ); // only one possible curve
            else {
                var selectedCurves = Selected.Select( x => x.Item1 ).Distinct(); // add to currently selected curve
                if( selectedCurves.Count() == 1 ) NewPoint( selectedCurves.First(), point );
                else { // need to create popup to pick
                    SavedPoint = point;
                    ImGui.OpenPopup( "NewPointPopup" );
                }
            }
        }

        private void NewPoint( AvfxCurveData curve, ImPlotPoint point ) {
            var time = Math.Round( point.x );
            var insertIdx = 0;
            foreach( var key in curve.Keys ) {
                if( key.DisplayX > time ) break;
                insertIdx++;
            }

            CommandManager.Add( new ListAddCommand<AvfxCurveKey>(
                curve.Keys,
                new AvfxCurveKey( curve, KeyType.Linear, ( int )time, 1, 1, IsColor ? 1.0f : ( float )curve.ToRadians( point.y ) ),
                insertIdx,
                ( AvfxCurveKey _, bool _ ) => OnUpdate()
            ) );
        }

        public void OnUpdate() {
            foreach( var curve in Curves.Where( x => x.IsAssigned() ) ) curve.Cleanup();
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
                [.. ColorCurve.Keys.Select( x => (x.Time.Value, x.Color))]
            ] );
        }

        private static uint Invert( Vector3 color ) =>
            color.X * 0.299 + color.Y * 0.587 + color.Z * 0.114 > 0.73 ? ImGui.GetColorU32( new Vector4( 0, 0, 0, 1 ) ) : ImGui.GetColorU32( new Vector4( 1, 1, 1, 1 ) );

        private static bool IsHovering() {
            var mousePos = ImGui.GetMousePos();
            var topLeft = ImPlot.GetPlotPos();
            var plotSize = ImPlot.GetPlotSize();
            if( mousePos.X >= topLeft.X && mousePos.X < topLeft.X + plotSize.X && mousePos.Y >= topLeft.Y && mousePos.Y < topLeft.Y + plotSize.Y ) return true;
            return false;
        }

        private void Copy() {
            CopiedKeys.Clear();
            foreach( var curve in AssignedCurves ) CopiedKeys.Add( curve.GetAvfxName(), [.. curve.Keys.Select( x => x.CopyPasteData )] );
        }

        private void Paste() => PerformOnCopiedKeys( ( List<ICommand> commands, List<(KeyType, Vector4)> keys, AvfxCurveData curve ) => {
            foreach( var key in keys ) {
                commands.Add( new ListAddCommand<AvfxCurveKey>( curve.Keys, new( curve, key ) ) );
            }
        } );

        private void Replace() => PerformOnCopiedKeys( ( List<ICommand> commands, List<(KeyType, Vector4)> keys, AvfxCurveData curve ) => {
            commands.Add( new ListSetCommand<AvfxCurveKey>( curve.Keys, [.. keys.Select( x => new AvfxCurveKey( curve, x ) )] ) );
        } );

        private void PerformOnCopiedKeys( Action<List<ICommand>, List<(KeyType, Vector4)>, AvfxCurveData> action ) {
            var commands = new List<ICommand>();
            if( CopiedKeys.Count == 0 || AssignedCurves.Count() == 0 ) return;
            if( CopiedKeys.Count == 1 && AssignedCurves.Count() == 1 ) {
                action( commands, CopiedKeys.First().Value, AssignedCurves.First() );
            }
            else {
                foreach( var (name, keys) in CopiedKeys ) {
                    if( !AssignedCurves.FindFirst( x => x.GetAvfxName() == name, out var curve ) ) continue;
                    action( commands, keys, curve );
                }
            }
            CommandManager.Add( new CompoundCommand( commands, OnUpdate ) );
        }

        private void Clear() {
            CommandManager.Add( new CompoundCommand( AssignedCurves.Select( curve => new ListSetCommand<AvfxCurveKey>( curve.Keys, [] ) ), OnUpdate ) );
        }
    }
}
