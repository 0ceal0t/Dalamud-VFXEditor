using ImGuiNET;
using VfxEditor.AVFXLib;
using VfxEditor.Data;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCheckbox : IUiBase {
        public readonly string Name;
        public bool Value;
        public readonly AVFXBool Literal;

        public UiCheckbox( string name, AVFXBool literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue() == true; // can be null
        }

        public void DrawInline( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXBool literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                Value = Literal.GetValue() == true;
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            if( ImGui.Checkbox( Name + id, ref Value ) ) {
                Literal.SetValue( Value );
            }

            if( IUiBase.DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
