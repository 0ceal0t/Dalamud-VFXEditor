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
    public class UIString : UIBase {
        public string Id;
        public LiteralString Literal;
        public string Value;
        public uint MaxSize;
        // ========================
        public delegate void Change(LiteralString literal);
        public Change ChangeFunction;

        public UIString(string id, LiteralString literal, Change changeFunction = null, int maxSizeBytes = 256) {
            Id = id;
            Literal = literal;
            Value = Literal.Value == null ? "" : Literal.Value;
            MaxSize = (uint)maxSizeBytes;
            ChangeFunction = changeFunction == null ? DoNothing : changeFunction;
        }

        public override void Draw(string id) {
            ImGui.InputText(Id + id, ref Value, MaxSize);
            ImGui.SameLine();
            if (ImGui.Button("Update" + id)) {
                Literal.GiveValue(Value.Trim('\0') + "\u0000");
                ChangeFunction(Literal);
            }
        }

        public static void DoNothing(LiteralString literal) { }
    }
}
