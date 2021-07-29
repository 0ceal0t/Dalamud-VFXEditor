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
    public class UIInt : UIBase
    {
        public string Id;
        public int Value;
        public LiteralInt Literal;

        public UIInt(string id, LiteralInt literal)
        {
            Id = id;
            Literal = literal;
            Value = Literal.Value;
        }

        public override void Draw(string id)
        {
            if (ImGui.InputInt(Id + id, ref Value))
            {
                Literal.GiveValue(Value);
            }
        }
    }
}
