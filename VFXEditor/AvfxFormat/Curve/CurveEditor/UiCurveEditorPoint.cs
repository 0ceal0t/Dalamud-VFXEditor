using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Numerics;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditorPoint {
        public static readonly KeyType[] KeyTypeOptions = ( KeyType[] )Enum.GetValues( typeof( KeyType ) );

        private readonly AvfxCurveKey Key;
        private readonly UiCurveEditor Editor;

        private readonly CurveType Type;
        private bool IsColor => Type == CurveType.Color;

        private bool IsColorChanging = false;
        private DateTime ColorChangeStartTime = DateTime.Now;
        private UiCurveEditorState PreColorChangeState;

        public KeyType KeyType => Key.Type;

        public Vector3 RawData {
            get => new( Key.X, Key.Y, ( float )ToDegrees( Key.Z ) );
            set {
                Key.X = value.X;
                Key.Y = value.Y;
                Key.Z = ( float )ToRadians( value.Z );
                Editor.UpdateGradient();
            }
        }

        public double DisplayX {
            get => Key.Time;
            set {
                Key.Time = ( int )Math.Max( 0, Math.Round( value ) );
            }
        }

        public double DisplayY {
            get => IsColor ? 0 : ToDegrees( Key.Z );
            set {
                if( !IsColor ) Key.Z = ( float )ToRadians( value );
                else Editor.UpdateGradient();
            }
        }

        public UiCurveEditorPoint( UiCurveEditor editor, AvfxCurveKey key, CurveType type ) {
            Editor = editor;
            Key = key;
            Type = type;
        }

        public Vector2 GetPosition() => new( ( float )DisplayX, ( float )DisplayY );

        public ImPlotPoint GetImPlotPoint() {
            return new ImPlotPoint {
                x = ( float )DisplayX,
                y = ( float )DisplayY
            };
        }

        public void Draw() {
            var id = "##CurveEdit";

            ImGui.PushStyleVar( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) );
            ImGui.PushFont( UiBuilder.IconFont );

            // Delete
            if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Editor.Keys.Remove( Key );
                    Editor.Points.Remove( this );
                    if( Editor.Selected.Contains( this ) ) Editor.Selected.Remove( this );
                    Editor.UpdateGradient();
                } ) );

                ImGui.PopStyleVar( 1 );
                ImGui.PopFont();
                return;
            }

            // Shift over left/right
            if( Editor.Points[0] != this ) {
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.ArrowLeft}{id}" ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx - 1];
                        Editor.Points[idx - 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateGradient();
                    } ) );
                }
            }
            if( Editor.Points[^1] != this ) {
                ImGui.SameLine();
                if( ImGui.Button( $"{( char )FontAwesomeIcon.ArrowRight}{id}" ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx + 1];
                        Editor.Points[idx + 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateGradient();
                    } ) );
                }
            }

            ImGui.PopStyleVar( 1 );
            ImGui.PopFont();

            // ====================

            var tempTime = Key.Time;
            if( ImGui.InputInt( "Time" + id, ref tempTime ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Time = tempTime;
                    Editor.UpdateGradient();
                } ) );
            }

            if( UiUtils.EnumComboBox( "Type" + id, KeyTypeOptions, Key.Type, out var newKeyType ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Type = newKeyType;
                } ) );
            }

            if( IsColor ) {
                var tempColorData = RawData;
                if( ImGui.ColorEdit3( "Color" + id, ref tempColorData, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                    if( !IsColorChanging ) {
                        IsColorChanging = true;
                        PreColorChangeState = Editor.GetState();
                    }
                    ColorChangeStartTime = DateTime.Now;
                    RawData = tempColorData;
                }
                else if( IsColorChanging && ( DateTime.Now - ColorChangeStartTime ).TotalMilliseconds > 200 ) {
                    IsColorChanging = false;
                    CommandManager.Avfx.Add( new UiCurveEditorDragCommand( Editor, PreColorChangeState, Editor.GetState() ) );
                }
            }
            else {
                var tempData = RawData;
                if( ImGui.InputFloat3( "Value" + id, ref tempData ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        RawData = tempData;
                    } ) );
                }
            }
        }

        private double ToRadians( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value; // no need to convert
            return ( Math.PI / 180 ) * value;
        }
        private double ToDegrees( double value ) {
            if( Type != CurveType.Angle || !Plugin.Configuration.UseDegreesForAngles ) return value; // no need to convert
            return ( 180 / Math.PI ) * value;
        }
    }
}
