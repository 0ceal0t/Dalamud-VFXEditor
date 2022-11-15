using ImGuiNET;
using ImPlotNET;
using System;
using System.Numerics;
using VfxEditor.Utils;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class UiCurveEditorPoint {
        public static readonly KeyType[] KeyTypeOptions = ( KeyType[] )Enum.GetValues( typeof( KeyType ) );

        public double X;
        public double Y;
        public bool Color;
        public Vector3 ColorData;
        public AVFXCurveKey Key;
        public UiCurveEditor Editor;

        private bool ColorDrag = false;
        private DateTime ColorDragTime = DateTime.Now;
        private UiCurveEditorState ColorDragState;

        public UiCurveEditorPoint( UiCurveEditor editor, AVFXCurveKey key, bool color = false ) {
            Editor = editor;
            Key = key;
            Color = color;

            X = key.Time;
            Y = Color ? 0 : key.Z;
            if( Color ) ColorData = new Vector3( Key.X, Key.Y, Key.Z );
        }

        public Vector2 GetPosition() => new( ( float )X, ( float )Y );

        public ImPlotPoint GetImPlotPoint() {
            return new ImPlotPoint {
                x = ( float )X,
                y = ( float )Y
            };
        }

        public void UpdateAvfxKeyPosition() {
            X = Math.Round( X ); // can only have integer time
            if( X < 0 ) X = 0;
            Key.Time = ( int )X;

            if( !Color ) Key.Z = ( float )Y;
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

            // Delete
            if( UiUtils.RemoveButton( "Delete Key" + id, small: true ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Editor.Keys.Remove( Key );
                    Editor.Points.Remove( this );
                    if( Editor.Selected == this ) Editor.Selected = null;
                    Editor.UpdateColor();
                } ) );
                return;
            }

            // Shift over left/right
            if( Editor.Points[0] != this ) {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Shift Left" + id ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx - 1];
                        Editor.Points[idx - 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateColor();
                    } ) );
                }
            }
            if( Editor.Points[^1] != this ) {
                ImGui.SameLine();
                if( ImGui.SmallButton( "Shift Right" + id ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx + 1];
                        Editor.Points[idx + 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateColor();
                    } ) );
                }
            }

            // Time
            var Time = Key.Time;
            if( ImGui.InputInt( "Time" + id, ref Time ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Time = Time;
                    X = Time;
                    Editor.UpdateColor();
                } ) );
            }

            // Type
            if( UiUtils.EnumComboBox( "Type" + id, KeyTypeOptions, Key.Type, out var newKeyType ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Type = newKeyType;
                } ) );
            }

            // Color/Data
            if( Color ) {
                if( ImGui.ColorEdit3( "Color" + id, ref ColorData, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                    if( !ColorDrag ) {
                        ColorDrag = true;
                        ColorDragState = Editor.GetState();
                    }
                    ColorDragTime = DateTime.Now;
                    UpdateColorData();
                }
                else if( ColorDrag && ( DateTime.Now - ColorDragTime ).TotalMilliseconds > 200 ) {
                    ColorDrag = false;
                    CommandManager.Avfx.Add( new UiCurveEditorDragCommand( Editor, ColorDragState, Editor.GetState() ) );
                }
            }
            else {
                var data = new Vector3( Key.X, Key.Y, Key.Z );
                if( ImGui.InputFloat3( "Value" + id, ref data ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        Key.X = data.X;
                        Key.Y = data.Y;
                        Key.Z = data.Z;
                        Y = Key.Z;
                    } ) );
                }
            }
        }
    }
}
