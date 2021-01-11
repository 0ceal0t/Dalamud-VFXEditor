using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;
using AVFXLib.Models;
using AVFXLib.Main;

namespace VFXEditor.UI.VFX
{
    public class UIString : UIBase
    {
        public string Id;
        public LiteralString Literal;
        public string Value;
        public uint MaxSize;
        // ========================
        public delegate void Change(LiteralString literal);
        public Change ChangeFunction;

        public UIString(string id, LiteralString literal, Change changeFunction = null, int maxSizeBytes = 256, string help = "" )
        {
            Id = id;
            Literal = literal;
            Value = Literal.Value;
            if(Value == null )
            {
                Value = "";
            }
            MaxSize = (uint)maxSizeBytes;
            if (changeFunction != null)
                ChangeFunction = changeFunction;
            else
                ChangeFunction = DoNothing;
            SetHelp( help );
        }

        public override void Draw(string id)
        {
            ImGui.InputText(Id + id, ref Value, MaxSize);
            ImGui.SameLine();
            if (ImGui.SmallButton("Update" + id))
            {
                Literal.GiveValue(Value.Trim('\0'));
                ChangeFunction(Literal);
            }
            DrawHelp();
        }

        public static void DoNothing(LiteralString literal) { }
    }
}
