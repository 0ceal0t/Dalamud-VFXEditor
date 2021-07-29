using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using Dalamud.Plugin;

namespace VFXEditor.UI.VFX
{
    public class UICheckbox : UIBase
    {
        public string Name;
        public bool Value;
        public LiteralBool Literal;

        public delegate void Change(LiteralBool literal);
        public Change ChangeFunction;

        public UICheckbox(string name, LiteralBool literal) {
            Name = name;
            Literal = literal;
            Value = (Literal.Value == true);
        }

        public override void Draw(string parentId) {
            if (ImGui.Checkbox( Name + parentId, ref Value)) {
                Literal.GiveValue(Value);
            }
        }
    }
}
