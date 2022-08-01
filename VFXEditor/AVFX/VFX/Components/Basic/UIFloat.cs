using ImGuiNET;
using VFXEditor.AVFXLib;
using VFXEditor.Data;

namespace VFXEditor.AVFX.VFX {
    public class UIFloat : UIBase {
        public readonly string Name;
        public float Value;
        public readonly AVFXFloat Literal;

        public UIFloat( string name, AVFXFloat literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue();
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) CopyManager.Copied[Name] = Literal;
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXFloat literal ) {
                Literal.SetValue( literal.GetValue() );
                Literal.SetAssigned( literal.IsAssigned() );
                Value = Literal.GetValue();
            }

            // Unassigned
            if( !Literal.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {Name}{id}" ) ) Literal.SetAssigned( true );
                return;
            }

            if( ImGui.InputFloat( Name + id, ref Value ) ) {
                Literal.SetValue( Value );
            }

            if( DrawUnassignContextMenu( id, Name ) ) Literal.SetAssigned( false );
        }
    }
}
