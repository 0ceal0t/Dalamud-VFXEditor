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
    public class UICombo<T> : UIBase
    {
        public string Id;
        public int ValueIdx;
        public LiteralEnum<T> Literal;

        public Action OnChange = null;

        public UICombo(string id, LiteralEnum<T> literal, Action onChange = null)
        {
            Id = id;
            Literal = literal;
            OnChange = onChange;
            var stringValue = Literal.Value.ToString();
            ValueIdx = Array.IndexOf( Literal.Options, stringValue );
        }

        public override void Draw(string id)
        {
            var text = ValueIdx == -1 ? "[NONE]" : Literal.Options[ValueIdx];
            if (ImGui.BeginCombo(Id + id, text ))
            {
                for ( var i = 0; i < Literal.Options.Length; i++)
                {
                    var isSelected = ( ValueIdx == i);
                    if (ImGui.Selectable(Literal.Options[i], isSelected))
                    {
                        ValueIdx = i;
                        Literal.GiveValue(Literal.Options[ValueIdx] );
                        OnChange?.Invoke();
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
