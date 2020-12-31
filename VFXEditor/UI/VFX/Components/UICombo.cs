using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;

namespace VFXEditor.UI.VFX
{
    public class UICombo<T> : UIBase
    {
        public string Id;
        public int Value;
        public LiteralEnum<T> Literal;

        public delegate void Change(LiteralEnum<T> literal);
        public Change ChangeFunction;

        public UICombo(string id, LiteralEnum<T> literal, Change changeFunction = null)
        {
            Id = id;
            Literal = literal;
            if (changeFunction != null)
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            // =====================
            Value = (int)(object)Literal.Value;
        }

        public override void Draw(string id)
        {
            if (ImGui.BeginCombo(Id + id, Literal.Options[Value]))
            {
                for (int i = 0; i < Literal.Options.Length; i++)
                {
                    bool isSelected = (Value == i);
                    if (ImGui.Selectable(Literal.Options[i], isSelected))
                    {
                        Value = i;
                        Literal.GiveValue(Literal.Options[Value]);
                        ChangeFunction(Literal);
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
        }

        public static void DoNothing(LiteralEnum<T> literal) { }
    }
}
