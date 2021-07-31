using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using VFXEditor.Data;

namespace VFXEditor.UI.VFX {
    public class UIFloat : UIBase {
        public string Name;
        public float Value;
        public LiteralFloat Literal;

        public UIFloat( string name, LiteralFloat literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.Value;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralFloat literal ) {
                Literal.GiveValue( literal.Value );
                Value = Literal.Value;
            }

            if( ImGui.InputFloat( Name + id, ref Value ) ) {
                Literal.GiveValue( Value );
            }
        }
    }
}
