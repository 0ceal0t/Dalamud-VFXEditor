using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using Dalamud.Plugin;
using VFXEditor.Data;

namespace VFXEditor.Avfx.Vfx {
    public class UIInt : UIBase {
        public string Name;
        public int Value;
        public LiteralInt Literal;

        public UIInt( string name, LiteralInt literal ) {
            Name = name;
            Literal = literal;
            Value = Literal.Value;
        }

        public override void Draw( string id ) {
            if( CopyManager.IsCopying ) {
                CopyManager.Copied[Name] = Literal;
            }
            if( CopyManager.IsPasting && CopyManager.Copied.TryGetValue( Name, out var b ) && b is LiteralInt literal ) {
                Literal.GiveValue( literal.Value );
                Value = Literal.Value;
            }

            if( ImGui.InputInt( Name + id, ref Value ) ) {
                Literal.GiveValue( Value );
            }
        }
    }
}
