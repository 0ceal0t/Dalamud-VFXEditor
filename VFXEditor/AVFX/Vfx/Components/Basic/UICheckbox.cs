using ImGuiNET;
using VFXEditor.AVFXLib;
using VFXEditor.Data;

namespace VFXEditor.AVFX.VFX {
    public class UICheckbox : UIBase {
        public readonly string Name;
        public bool Value;
        public readonly AVFXBool Literal;

        public UICheckbox( string name, AVFXBool literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue() == true; // can be null
        }

        public override void Draw( string parentId ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }

            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXBool literal ) {
                Literal.SetValue( literal.GetValue() );
                Value = ( Literal.GetValue() == true );
            }

            PushAssignedColor( Literal.IsAssigned() );
            if( ImGui.Checkbox( Name + parentId, ref Value ) ) {
                Literal.SetValue( Value );
            }
            PopAssignedColor();
        }
    }
}
