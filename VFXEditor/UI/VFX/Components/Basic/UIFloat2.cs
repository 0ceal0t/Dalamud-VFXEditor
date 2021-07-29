using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using System.Numerics;

namespace VFXEditor.UI.VFX
{
    public class UIFloat2 : UIBase
    {
        public string Id;
        public Vector2 Value;

        public LiteralFloat Literal1;
        public LiteralFloat Literal2;

        public UIFloat2(string id, LiteralFloat literal1, LiteralFloat literal2)
        {
            Id = id;
            Literal1 = literal1;
            Literal2 = literal2;

            Value = new Vector2(Literal1.Value, Literal2.Value);
        }

        public override void Draw(string id)
        {
            if (ImGui.InputFloat2(Id + id, ref Value))
            {
                Literal1.GiveValue(Value.X);
                Literal2.GiveValue(Value.Y);
            }
        }
    }
}
