using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;

namespace VFXEditor.UI.VFX
{
    public class UICheckbox : UIBase
    {
        public string Id;
        public bool Value;
        public LiteralBool Literal;

        public delegate void Change(LiteralBool literal);
        public Change ChangeFunction;

        public int SL;

        public UICheckbox(string id, LiteralBool literal, Change changeFunction = null, int sl = 0)
        {
            Id = id;
            Literal = literal;
            SL = sl;
            if (changeFunction != null)
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            // =====================
            Value = (Literal.Value == true);
        }

        public override void Draw(string id)
        {
            if (SL > 0) ImGui.SameLine(SL);
            if (ImGui.Checkbox(Id + id, ref Value))
            {
                Literal.GiveValue(Value);
                ChangeFunction(Literal);
            }
        }

        public static void DoNothing(LiteralBool literal) {}
    }
}
