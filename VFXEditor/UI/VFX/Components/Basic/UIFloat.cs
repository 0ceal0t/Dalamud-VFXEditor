using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;

namespace VFXEditor.UI.VFX
{
    public class UIFloat : UIBase
    {
        public string Id;
        public float Value;
        public LiteralFloat Literal;

        public UIFloat(string id, LiteralFloat literal)
        {
            Id = id;
            Literal = literal;
            Value = Literal.Value;
        }

        public override void Draw(string id)
        {
            if (ImGui.InputFloat(Id + id, ref Value))
            {
                Literal.GiveValue(Value);
            }
        }
    }
}
