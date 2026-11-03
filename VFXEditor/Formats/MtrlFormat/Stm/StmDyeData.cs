using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/e93093f/Files/StainMapStructs/DyePack.cs

    public class StmDyeData : StmDyeDataBase {
        public float Exposure = 0;
        public float Metalness = 0;
        public float Roughness = 0;
        public float SheenRate = 0;
        public float SheenTintRate = 0;
        public float SheenAperture = 0;
        public float Anisotropy = 0;
        public int SphereMapIndex = 0;
        public float SphereMapMask = 0;

        protected override void DrawScalars() {
            using var disabled = ImRaii.Disabled();
            var hovered = false;

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 125 );
            ImGui.InputFloat( "##Exposure", ref Exposure, 0, 0, $"Exposure: %.1f", ImGuiInputTextFlags.ReadOnly );
            hovered |= ImGui.IsItemHovered();

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 125 );
            ImGui.InputFloat( "##Metalness", ref Metalness, 0, 0, $"Metalness: %.1f", ImGuiInputTextFlags.ReadOnly );
            hovered |= ImGui.IsItemHovered();

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 125 );
            ImGui.InputFloat( "##Roughness", ref Roughness, 0, 0, $"Roughness: %.1f", ImGuiInputTextFlags.ReadOnly );
            hovered |= ImGui.IsItemHovered();
        }
    }
}