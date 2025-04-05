using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Formats.AvfxFormat.Curve.Lines;
using VfxEditor.Parsing;
using VfxEditor.Utils;
using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxCurveKey {
        public static readonly KeyType[] KeyTypeOptions = Enum.GetValues<KeyType>();
        private readonly AvfxCurveData Curve;
        private bool IsColor => Curve.IsColor;

        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private Vector3 ColorBeforeEdit;
        private Vector3 BackupColor;

        public readonly ParsedEnum<KeyType> Type = new( "Type", 2 );
        public readonly ParsedShort Time = new( "Time" );
        public readonly ParsedFloat3 Data = new( "Data" );

        public Vector3 Converted => new( Data.Value.X, Data.Value.Y, ( float )Curve.ToDegrees( Data.Value.Z ) );
        public Vector3 Color => Data.Value;
        public ImPlotPoint Point => new() {
            x = ( float )DisplayX,
            y = ( float )DisplayY
        };
        public (KeyType, Vector4) CopyPasteData => (Type.Value, new Vector4( Time.Value, Data.Value.X, Data.Value.Y, Data.Value.Z ));

        public double DisplayX {
            get => Time.Value;
            set {
                Time.Value = ( int )Math.Max( 0, Math.Round( value ) );
            }
        }
        public double DisplayY {
            get => IsColor ? 0 : Curve.ToDegrees( Data.Value.Z );
            set {
                if( !IsColor ) Data.Value = Data.Value with { Z = ( float )Curve.ToRadians( value ) };
            }
        }

        private (int, Vector3) StateBeforeEditing;

        public AvfxCurveKey( AvfxCurveData curve ) {
            Curve = curve;
        }

        public AvfxCurveKey( AvfxCurveData curve, (KeyType, Vector4) copyPaste ) : this( curve ) {
            Type.Value = copyPaste.Item1;
            var data = copyPaste.Item2;
            Time.Value = ( int )data.X;
            Data.Value = new( data.Y, data.Z, data.W );
        }

        public AvfxCurveKey( AvfxCurveData curve, KeyType type, int time, float x, float y, float z ) : this( curve ) {
            Type.Value = type;
            Time.Value = time;
            Data.Value = new( x, y, z );
        }

        public AvfxCurveKey( AvfxCurveData curve, BinaryReader reader ) : this( curve ) {
            Time.Read( reader );
            Type.Read( reader );
            Data.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Time.Write( writer );
            Type.Write( writer );
            Data.Write( writer );
        }

        // ======= EDITING =======

        public void StartDragging() {
            StateBeforeEditing = (Time.Value, Data.Value);
        }

        public void StopDragging( List<ICommand> commands ) {
            commands.Add( new ParsedSimpleCommand<int>( Time, StateBeforeEditing.Item1, Time.Value ) );
            commands.Add( new ParsedSimpleCommand<Vector3>( Data, StateBeforeEditing.Item2, Data.Value ) );
        }

        // ======= DRAWING =========

        public void Draw( LineEditorGroup editor ) {
            Type.OnChangeAction = editor.OnUpdate;
            Time.OnChangeAction = editor.OnUpdate;
            // Don't need to bother with X, Y, Z since they have their own weird inputs

            using var _ = ImRaii.PushId( "Key" );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                // Delete
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    CommandManager.Add( new ListRemoveCommand<AvfxCurveKey>( Curve.Keys, this, ( AvfxCurveKey _, bool _ ) => editor.OnUpdate() ) );
                    return;
                }

                // Duplicate
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Copy.ToIconString() ) ) {
                    var newKey = new AvfxCurveKey( Curve, Type.Value, Time.Value + 1, Data.Value.X, Data.Value.Y, Data.Value.Z );
                    CommandManager.Add( new ListAddCommand<AvfxCurveKey>( Curve.Keys, newKey, Curve.Keys.IndexOf( this ) + 1, ( AvfxCurveKey _, bool _ ) => editor.OnUpdate() ) );
                }

                // Shift left/right
                ImGui.SameLine();
                if( UiUtils.DisabledButton( FontAwesomeIcon.ArrowLeft.ToIconString(), !( Curve.Keys.Count == 0 || Curve.Keys[0] == this ) ) ) {
                    CommandManager.Add( new ListMoveCommand<AvfxCurveKey>( Curve.Keys, this, Curve.Keys[Curve.Keys.IndexOf( this ) - 1], ( AvfxCurveKey _ ) => editor.OnUpdate() ) );
                }
                ImGui.SameLine();
                if( UiUtils.DisabledButton( FontAwesomeIcon.ArrowRight.ToIconString(), !( Curve.Keys.Count == 0 || Curve.Keys[^1] == this ) ) ) {
                    CommandManager.Add( new ListMoveCommand<AvfxCurveKey>( Curve.Keys, this, Curve.Keys[Curve.Keys.IndexOf( this ) + 1], ( AvfxCurveKey _ ) => editor.OnUpdate() ) );
                }
            }

            Time.Draw();
            Type.Draw();

            if( IsColor ) DrawColor( editor );
            else {
                var data = Converted;
                if( ImGui.InputFloat3( "Value", ref data ) ) {
                    CommandManager.Add( new CompoundCommand( new[] {
                        new ParsedSimpleCommand<Vector3>( Data, new Vector3( data.X, data.Y,(float)Curve.ToRadians( data.Z ) )  )
                    }, onChangeAction: editor.OnUpdate ) );
                }
            }
        }

        // ======== DRAW COLOR ============

        private void DrawColor( LineEditorGroup editor ) {
            var changed = false;
            var color = Color;
            var prevColor = Color;

            var imguiStyle = ImGui.GetStyle();
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( imguiStyle.ItemInnerSpacing.X, imguiStyle.ItemSpacing.Y ) ) ) {
                ImGui.SetNextItemWidth( ImGui.GetWindowSize().X * 0.65f - imguiStyle.ItemInnerSpacing.X - ImGui.GetFrameHeight() );
                if( ImGui.ColorEdit3( "##ColorInput", ref color, ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview | ImGuiColorEditFlags.Float ) ) {
                    changed = true;
                    Data.Value = color;
                }
                ImGui.SameLine();
                if( ImGui.ColorButton( "##ColorButton", new Vector4( color, 1 ) ) ) {
                    BackupColor = color;
                    ImGui.OpenPopup( "PalettePopup" );
                }

                ImGui.SameLine();
                ImGui.Text( "Color" );
            }

            if( DrawPalettePopup( color, BackupColor, out var popupColor ) ) {
                changed = true;
                Data.Value = popupColor;
            }

            if( changed ) {
                if( !Editing ) {
                    Editing = true;
                    ColorBeforeEdit = prevColor;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) { // Only actually commit the changes if 200ms have passed since the last one
                Editing = false;
                CommandManager.Add( new CompoundCommand( new[] {
                    new ParsedSimpleCommand<Vector3>( Data, ColorBeforeEdit, color )
                }, onChangeAction: editor.OnUpdate ) );
            }
        }

        // ======== PALETTE ============

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

            for( var idx = 0; idx < Plugin.Configuration.CurveEditorPalette.Count; idx++ ) {
                using var _ = ImRaii.PushId( idx );
                var paletteColor = Plugin.Configuration.CurveEditorPalette[idx];

                if( ( idx % 8 ) != 0 ) ImGui.SameLine( 0f, ImGui.GetStyle().ItemSpacing.Y );

                if( ImGui.ColorButton( "##Palette", paletteColor, ImGuiColorEditFlags.NoAlpha | ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip ) ) {
                    color = new Vector3( paletteColor.X, paletteColor.Y, paletteColor.Z );
                    ret = true;
                }

                if( ImGui.BeginDragDropTarget() ) {
                    var payload3 = ImGui.AcceptDragDropPayload( "_COL3F" );
                    if( payload3.NativePtr != null ) {
                        Plugin.Configuration.CurveEditorPalette[idx] = new Vector4( *( Vector3* )payload3.Data, 1 );
                        Plugin.Configuration.Save();
                    }

                    var payload4 = ImGui.AcceptDragDropPayload( "_COL4F" );
                    if( payload4.NativePtr != null ) {
                        Plugin.Configuration.CurveEditorPalette[idx] = *( Vector4* )payload4.Data;
                        Plugin.Configuration.Save();
                    }

                    ImGui.EndDragDropTarget();
                }
            }

            return ret;
        }
    }
}
