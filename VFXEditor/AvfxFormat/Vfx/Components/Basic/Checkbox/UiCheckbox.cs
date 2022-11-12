using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCheckbox : IUiBase {
        public readonly string Name;
        public readonly AVFXBool Literal;

        public UiCheckbox( string name, AVFXBool literal ) {
            Name = name;
            Literal = literal;
        }

        public void DrawInline( string id ) {
            // Copy/Paste
            if( CopyManager.IsCopying ) {
                CopyManager.Assigned[Name] = Literal.IsAssigned();
                CopyManager.Bools[Name] = Literal.GetValue() == true;
            }
            if( CopyManager.IsPasting ) {
                if( CopyManager.Assigned.TryGetValue(Name, out var a ) ) CopyManager.PasteCommand.Add( new UiAssignableCommand( Literal, a ) );
                if( CopyManager.Bools.TryGetValue( Name, out var l ) ) CopyManager.PasteCommand.Add( new UiCheckboxCommand( Literal, l ) );
            }

            // Unassigned
            if( IUiBase.DrawAddButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue() == true;
            if( ImGui.Checkbox( Name + id, ref value ) ) CommandManager.Avfx.Add( new UiCheckboxCommand( Literal, value ) );

            IUiBase.DrawRemoveContextMenu( Literal, Name, id );
        }
    }
}
