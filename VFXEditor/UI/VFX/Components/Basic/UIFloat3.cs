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
    public class UIFloat3 : UIBase
    {
        public string Id;
        public Vector3 Value;

        public LiteralFloat Literal1;
        public LiteralFloat Literal2;
        public LiteralFloat Literal3;

        public delegate void Change(LiteralFloat literal1, LiteralFloat literal2, LiteralFloat literal3);
        public Change ChangeFunction;

        public UIFloat3(string id, LiteralFloat literal1, LiteralFloat literal2, LiteralFloat literal3, Change changeFunction = null, string help = "" )
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
                Literal1.GiveValue(Value.X);
                Literal2.GiveValue(Value.Y);
                Literal3.GiveValue(Value.Z);
                ChangeFunction(Literal1, Literal2, Literal3);
            }
            DrawHelp();
        }

        public static void DoNothing(LiteralFloat literal1, LiteralFloat literal2, LiteralFloat literal3) { }
    }
}
