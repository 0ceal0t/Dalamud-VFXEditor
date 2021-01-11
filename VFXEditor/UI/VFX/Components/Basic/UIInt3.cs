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
    public class UIInt3 : UIBase
    {
        public string Id;
        public Vector3 Value;

        public LiteralInt Literal1;
        public LiteralInt Literal2;
        public LiteralInt Literal3;

        public delegate void Change(LiteralInt literal1, LiteralInt literal2, LiteralInt literal3);
        public Change ChangeFunction;

        public UIInt3(string id, LiteralInt literal1, LiteralInt literal2, LiteralInt literal3, Change changeFunction = null, string help = "" )
        {
            Id = id;
            Literal1 = literal1;
            Literal2 = literal2;
            Literal3 = literal3;
            if (changeFunction != null)
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            // =====================
            Value = new Vector3(Literal1.Value, Literal2.Value, Literal3.Value);
            SetHelp( help );
        }

        public override void Draw(string id)
        {
            if (ImGui.InputFloat3(Id + id, ref Value))
            {
                Literal1.GiveValue((int)Value.X);
                Literal2.GiveValue((int)Value.Y);
                Literal3.GiveValue((int)Value.Z);
                ChangeFunction(Literal1, Literal2, Literal3);
            }
            DrawHelp();
        }

        public static void DoNothing(LiteralInt literal1, LiteralInt literal2, LiteralInt literal3) { }
    }
}
