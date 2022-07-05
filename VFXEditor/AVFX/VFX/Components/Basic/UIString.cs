using ImGuiNET;
using VFXEditor.AVFXLib;
using VFXEditor.Data;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
    public class UIString : UIBase {
        public readonly string Name;
        public readonly AVFXString Literal;
        public string Value;
        public readonly bool CanBeUnassigned;

        public UIString( string name, AVFXString literal, bool canBeUnassigned = false ) {
            Name = name;
            Literal = literal;
            Value = Literal.GetValue() ?? "";
            CanBeUnassigned = canBeUnassigned;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }

            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var _literal ) && _literal is AVFXString literal ) {
                Literal.SetValue( literal.GetValue() );
                Value = Literal.GetValue() ?? "";
            }

            if( CanBeUnassigned ) {
                if( Literal.IsAssigned() && UIHelper.RemoveButton( $"Remove {Name}{id}", small: true ) ) {
                    Value = "";
                    Literal.SetValue( "" );
                    Literal.SetAssigned( false );
                }
                else if( !Literal.IsAssigned() && UIHelper.RemoveButton( $"Add {Name}{id}", small: true ) ) {
                    Value = "";
                    Literal.SetValue( "" );
                    Literal.SetAssigned( true );
                }

                if( !Literal.IsAssigned() ) return;
            }

            PushAssignedColor( Literal.IsAssigned() );
            ImGui.InputText( Name + id, ref Value, 256 );
            PopAssignedColor();

            ImGui.SameLine();
            if( ImGui.Button( "Update" + id ) ) {
                Literal.SetValue( Value.Trim().Trim( '\0' ) + '\u0000' );
                if( CanBeUnassigned && Literal.GetValue().Trim( '\0' ).Length == 0 ) {
                    Literal.SetAssigned( false );
                }
            }
        }
    }
}
