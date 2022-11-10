using Dalamud.Interface;
using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiString : IUiBase {
        public readonly string Name;
        public readonly AVFXString Literal;
        public string Value;
        public readonly bool ShowRemoveButton;

        public UiString( string name, AVFXString literal, bool showRemoveButton = false ) {
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

            var style = ImGui.GetStyle();
            var spacing = 2;
            ImGui.PushFont( UiBuilder.IconFont );
            var checkSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Check}" ).X + style.FramePadding.X * 2 + spacing;
            var removeSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Trash}" ).X + style.FramePadding.X * 2 + spacing;
            ImGui.PopFont();

            var inputSize = ImGui.GetContentRegionAvail().X * 0.65f - checkSize - ( ShowRemoveButton ? removeSize : 0);
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( $"{id}-MainInput", ref Value, 256 );

            if( IUiBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );

            ImGui.PushFont( UiBuilder.IconFont );

            ImGui.SameLine(inputSize + spacing );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                Literal.SetValue( Value.Trim().Trim( '\0' ) + '\u0000' );
                if( ShowRemoveButton && Literal.GetValue().Trim( '\0' ).Length == 0 ) {
                    Literal.SetAssigned( false );
                }
            }

            if( ShowRemoveButton ) {
                ImGui.SameLine(inputSize + checkSize + spacing);
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id ) ) {
                    Value = "";
                    Literal.SetValue( "" );
                    Literal.SetAssigned( false );
                }
            }

            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
