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
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXBool literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
            }

            // Unassigned
            if( IUiBase.DrawCommandButton( Literal, Name, id ) ) return;

            var value = Literal.GetValue() == true;
            if( ImGui.Checkbox( Name + id, ref value ) ) CommandManager.Avfx.Add( new UiCheckboxCommand( Literal, value ) );

            IUiBase.DrawCommandContextMenu( Literal, Name, id );
        }
    }
}
