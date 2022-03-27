using System;
using ImGuiNET;
using AVFXLib.Models;
using VFXEditor.Data;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UIString : UIBase {
        public string Name;
        public LiteralString Literal;
        public string Value;
        public bool CanBeUnassigned;

        public UIString( string name, LiteralString literal, bool canBeUnassigned = false ) {
            Name = name;
            Literal = literal;
            Value = Literal.Value ?? "";
            CanBeUnassigned = canBeUnassigned;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralString literal ) {
                Literal.GiveValue( literal.Value );
                Value = Literal.Value ?? "";
            }

            if (CanBeUnassigned) {
                if (Literal.Assigned && UiHelper.RemoveButton( $"Remove {Name}{id}", small: true ) ) {
                    Value = "";
                    Literal.GiveValue( "" );
                    Literal.Assigned = false;
                }
                else if( !Literal.Assigned && UiHelper.RemoveButton( $"Add {Name}{id}", small: true ) ) {
                    Value = "";
                    Literal.GiveValue( "" );
                    Literal.Assigned = true;
                }

                if( !Literal.Assigned ) return;
            }

            ImGui.InputText( Name + id, ref Value, 256 );
            ImGui.SameLine();
            if( ImGui.Button( "Update" + id ) ) {
                Literal.GiveValue( Value.Trim().Trim( '\0' ) + "\u0000" );
                if( CanBeUnassigned && Literal.Value.Trim( '\0' ).Length == 0 ) {
                    Literal.Assigned = false;
                }
            }
        }
    }
}
