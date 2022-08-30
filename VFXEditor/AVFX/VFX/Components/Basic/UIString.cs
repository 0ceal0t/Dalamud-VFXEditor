using Dalamud.Interface;
using ImGuiNET;
using VFXEditor.AVFXLib;
using VFXEditor.Data;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public class UIString : IUIBase {
        public readonly string Name;
        public readonly AVFXString Literal;
        public string Value;
        public readonly bool ShowRemoveButton;

        public UIString( string name, AVFXString literal, bool showRemoveButton = false ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue() ?? "";
            ShowRemoveButton = showRemoveButton;
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXString literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                Value = Literal.GetValue() ?? "";
            }

            // Unassigned
            if (!Literal.IsAssigned()) {
                if (ImGui.SmallButton($"+ {Name}{id}")) {
                    Literal.SetAssigned( true );
                }
                return;
            }

            ImGui.InputText( Name + id, ref Value, 256 );

            if( IUIBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );

            ImGui.SameLine();
            if( ImGui.Button( $"Update{id}" ) ) {
                Literal.SetValue( Value.Trim().Trim( '\0' ) + '\u0000' );
                if( ShowRemoveButton && Literal.GetValue().Trim( '\0' ).Length == 0 ) {
                    Literal.SetAssigned( false );
                }
            }

            if( ShowRemoveButton ) {
                ImGui.SameLine();
                ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );

                ImGui.PushFont( UiBuilder.IconFont );
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id ) ) {
                    Value = "";
                    Literal.SetValue( "" );
                    Literal.SetAssigned( false );
                }
                ImGui.PopFont();
            }
        }
    }
}
