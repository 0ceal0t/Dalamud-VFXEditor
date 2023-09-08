using Dalamud.Interface;
using ImGuiNET;
using ImPlotNET;
using OtterGui.Raii;
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

        private Vector3 BackupColor;

        public Vector3 RawData {
            get => new( Key.X, Key.Y, ( float )Editor.ToDegrees( Key.Z ) );
            set {
                Key.X = value.X;
                Key.Y = value.Y;
                Key.Z = ( float )Editor.ToRadians( value.Z );
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
            get => IsColor ? 0 : Editor.ToDegrees( Key.Z );
            set {
                if( !IsColor ) Key.Z = ( float )Editor.ToRadians( value );
                else Editor.UpdateGradient();
            }
        }

        public UiCurveEditorPoint( UiCurveEditor editor, AvfxCurveKey key, CurveType type ) {
            Editor = editor;
            Key = key;
            Type = type;
        }

        public ImPlotPoint GetImPlotPoint() {
            return new ImPlotPoint {
                x = ( float )DisplayX,
                y = ( float )DisplayY
            };
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "CurveEdit" );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 4, 4 ) ) ) {
                // Delete
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        Editor.Keys.Remove( Key );
                        Editor.Points.Remove( this );
                        if( Editor.Selected.Contains( this ) ) Editor.Selected.Remove( this );
                        Editor.UpdateGradient();
                    } ) );
                    return;
                }

                // shift over left/right

                var leftDisabled = Editor.Points.Count == 0 || Editor.Points[0] == this;
                var rightDisabled = Editor.Points.Count == 0 || Editor.Points[^1] == this;

                ImGui.SameLine();
                if( UiUtils.DisabledButton( FontAwesomeIcon.ArrowLeft.ToIconString(), !leftDisabled ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx - 1];
                        Editor.Points[idx - 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateGradient();
                    } ) );
                }

                ImGui.SameLine();
                if( UiUtils.DisabledButton( FontAwesomeIcon.ArrowRight.ToIconString(), !rightDisabled ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        var idx = Editor.Points.IndexOf( this );
                        var swap = Editor.Points[idx + 1];
                        Editor.Points[idx + 1] = this;
                        Editor.Points[idx] = swap;
                        Editor.UpdateGradient();
                    } ) );
                }
            }

            var tempTime = Key.Time;
            if( ImGui.InputInt( "Time", ref tempTime ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Time = tempTime;
                    Editor.UpdateGradient();
                } ) );
            }

            if( UiUtils.EnumComboBox( "Type", KeyTypeOptions, Key.Type, out var newKeyType ) ) {
                CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                    Key.Type = newKeyType;
                } ) );
            }

            if( IsColor ) {
                var tempX = RawData.X;
                var tempY = RawData.Y;
                var tempZ = RawData.Z;
                var tempColor = RawData;

                var changed = false;

                var imguiStyle = ImGui.GetStyle();
                var floatWidth = ( ImGui.GetWindowSize().X * 0.65f - imguiStyle.ItemInnerSpacing.X * 3 - ImGui.GetFrameHeight() ) / 3f;
                using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) ) ) {
                    ImGui.SetNextItemWidth( floatWidth );
                    if( ImGui.DragFloat( "##X", ref tempX, 1.0f / 255.0f, 0.0f, 1.0f, "R:%0.3f" ) ) {
                        changed = true;
                        tempColor = RawData with {
                            X = tempX
                        };
                    }

                    ImGui.SameLine();
                    ImGui.SetNextItemWidth( floatWidth );
                    if( ImGui.DragFloat( "##Y", ref tempY, 1.0f / 255.0f, 0.0f, 1.0f, "G:%0.3f" ) ) {
                        changed = true;
                        tempColor = RawData with {
                            Y = tempY
                        };
                    }

                    ImGui.SameLine();
                    ImGui.SetNextItemWidth( floatWidth );
                    if( ImGui.DragFloat( "##Z", ref tempZ, 1.0f / 255.0f, 0.0f, 1.0f, "B:%0.3f" ) ) {
                        changed = true;
                        tempColor = RawData with {
                            Z = tempZ
                        };
                    }

                    ImGui.SameLine();
                    if( ImGui.ColorButton( "##Color", new Vector4( tempColor, 1 ) ) ) {
                        BackupColor = tempColor;
                        ImGui.OpenPopup( "PalettePopup" );
                    }

                    ImGui.SameLine();
                    ImGui.Text( "Color" );
                }

                if( DrawPalettePopup( tempColor, BackupColor, out var popupColor ) ) {
                    changed = true;
                    tempColor = popupColor;
                }

                if( changed ) {
                    if( !IsColorChanging ) {
                        IsColorChanging = true;
                        PreColorChangeState = Editor.GetState();
                    }
                    ColorChangeStartTime = DateTime.Now;
                    RawData = tempColor;
                }
                else if( IsColorChanging && ( DateTime.Now - ColorChangeStartTime ).TotalMilliseconds > 200 ) {
                    IsColorChanging = false;
                    CommandManager.Avfx.Add( new UiCurveEditorDragCommand( Editor, PreColorChangeState, Editor.GetState() ) );
                }
            }
            else {
                var tempData = RawData;
                if( ImGui.InputFloat3( "Value", ref tempData ) ) {
                    CommandManager.Avfx.Add( new UiCurveEditorCommand( Editor, () => {
                        RawData = tempData;
                    } ) );
                }
            }
        }

        private static unsafe bool DrawPalettePopup( Vector3 currentColor, Vector3 backupColor, out Vector3 color ) {
            color = currentColor;
            var ret = false;

            using var popup = ImRaii.Popup( "PalettePopup" );
            if( !popup ) return false;

            if( ImGui.ColorPicker3( "##Picker", ref currentColor, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview ) ) {
                color = currentColor;
                ret = true;
            }

            ImGui.SameLine();

            using var group = ImRaii.Group();

            ImGui.Text( "Current" );
            ImGui.ColorButton( "##Current", new Vector4( currentColor, 1 ), ImGuiColorEditFlags.NoPicker, new Vector2( 60, 40 ) );

            ImGui.Text( "Previous" );
            if( ImGui.ColorButton( "##Previous", new Vector4( backupColor, 1 ), ImGuiColorEditFlags.NoPicker, new Vector2( 60, 40 ) ) ) {
                color = backupColor;
                ret = true;
            }

            ImGui.Separator();

            for( var i = 0; i < Plugin.Configuration.CurveEditorPalette.Count; i++ ) {
                using var _ = ImRaii.PushId( i );
                var paletteColor = Plugin.Configuration.CurveEditorPalette[i];

                if( ( i % 8 ) != 0 ) ImGui.SameLine( 0f, ImGui.GetStyle().ItemSpacing.Y );

                if( ImGui.ColorButton( "##Palette", paletteColor, ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip ) ) {
                    color = new Vector3( paletteColor.X, paletteColor.Y, paletteColor.Z );
                    ret = true;
                }

                if( ImGui.BeginDragDropTarget() ) {
                    var payload3 = ImGui.AcceptDragDropPayload( "_COL3F" );
                    if( payload3.NativePtr != null ) {
                        var vec3 = *( Vector3* )payload3.Data;
                        Plugin.Configuration.CurveEditorPalette[i] = new Vector4( vec3, 1 );
                        Plugin.Configuration.Save();
                    }

                    var payload4 = ImGui.AcceptDragDropPayload( "_COL4F" );
                    if( payload4.NativePtr != null ) {
                        var vec4 = *( Vector4* )payload4.Data;
                        Plugin.Configuration.CurveEditorPalette[i] = vec4;
                        Plugin.Configuration.Save();
                    }

                    ImGui.EndDragDropTarget();
                }
            }

            return ret;
        }
    }
}
