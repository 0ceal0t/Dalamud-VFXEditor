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

        public delegate void Change(LiteralFloat literal);
        public Change ChangeFunction;

        public UIFloat(string id, LiteralFloat literal, Change changeFunction = null, string help = "" )
        {
            Id = id;
            Literal = literal;
            if (changeFunction != null)
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            // =====================
            Value = Literal.Value;
            SetHelp( help );
        }

        public override void Draw(string id)
        {
            if (ImGui.InputFloat(Id + id, ref Value))
            {
                Literal.GiveValue(Value);
                ChangeFunction(Literal);
            }
            DrawHelp();
        }

        public static void DoNothing(LiteralFloat literal) { }
    }
}
