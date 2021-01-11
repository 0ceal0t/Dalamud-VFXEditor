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

        public delegate void Change(LiteralInt literal);
        public Change ChangeFunction;

        public UIInt(string id, LiteralInt literal, Change changeFunction = null, string help = "" )
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
            if (ImGui.InputInt(Id + id, ref Value))
            {
                Literal.GiveValue(Value);
                ChangeFunction(Literal);
            }
            DrawHelp();
        }

        public static void DoNothing(LiteralInt literal) { }
    }
}
