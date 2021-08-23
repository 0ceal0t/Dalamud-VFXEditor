using ImGuiNET;
using AVFXLib.Models;
using VFXEditor.Data;

namespace VFXEditor.UI.VFX {
    public class UICheckbox : UIBase {
        public string Name;
        public bool Value;
        public LiteralBool Literal;

        public UICheckbox( string name, LiteralBool literal ) {
            Name = name;
            Literal = literal;
            Value = ( Literal.Value == true );
        }

        public override void Draw( string parentId ) {
            if(CopyManager.IsCopying) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue(Name, out var b) && b is LiteralBool literal ) {
                Literal.GiveValue( literal.Value );
                Value = ( Literal.Value == true );
            }

            if( ImGui.Checkbox( Name + parentId, ref Value ) ) {
                Literal.GiveValue( Value );
            }
        }
    }
}
