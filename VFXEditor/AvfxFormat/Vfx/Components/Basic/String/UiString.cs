using Dalamud.Interface;
using ImGuiNET;
using Newtonsoft.Json.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiString : IUiBase {
        public readonly string Name;
        public readonly AVFXString Literal;
        public string InputString;
        public readonly bool ShowRemoveButton;

        public UiString( string name, AVFXString literal, bool showRemoveButton = false ) {
            Name = name;
            Literal = literal;
            InputString = Literal.GetValue() ?? "";
            ShowRemoveButton = showRemoveButton;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Strings[Name] = Literal.GetValue();
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue( Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Strings.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiStringCommand( this, l, !a ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var style = ImGui.GetStyle();
            var spacing = 2;
            ImGui.PushFont( UiBuilder.IconFont );
            var checkSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Check}" ).X + style.FramePadding.X * 2 + spacing;
            var removeSize = ImGui.CalcTextSize( $"{( char )FontAwesomeIcon.Trash}" ).X + style.FramePadding.X * 2 + spacing;
            ImGui.PopFont();

            var inputSize = ImGui.GetContentRegionAvail().X * 0.65f - checkSize - ( ShowRemoveButton ? removeSize : 0);
            ImGui.SetNextItemWidth( inputSize );
            ImGui.InputText( $"{id}-MainInput", ref InputString, 256 );

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );

            ImGui.PushFont( UiBuilder.IconFont );

            ImGui.SameLine(inputSize + spacing );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Check}" + id ) ) {
                var newValue = InputString.Trim().Trim( '\0' ) + '\u0000';
                CommandManager.Avfx.Add( new UiStringCommand( this, newValue, ShowRemoveButton && newValue.Trim('\0').Length == 0 ) );
            }

            if( ShowRemoveButton ) {
                ImGui.SameLine(inputSize + checkSize + spacing);
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}" + id ) ) {
                    CommandManager.Avfx.Add( new UiStringCommand( this, "", true ) );
                }
            }

            ImGui.PopFont();

            ImGui.SameLine();
            ImGui.Text( Name );
        }
    }
}
