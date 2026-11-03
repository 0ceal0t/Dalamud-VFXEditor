using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public class StmDyeDataLegacy : StmDyeDataBase {
        public float Gloss = 0;
        public float Power = 0;

        protected override void DrawScalars() {
            using var disabled = ImRaii.Disabled();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( "##Gloss", ref Gloss, 0, 0, $"Gloss: %.1f", ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( "##Power", ref Power, 0, 0, $"Power: %.1f", ImGuiInputTextFlags.ReadOnly );
        }
    }
}