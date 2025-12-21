using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public class StmDyeData {
        public Vector3 Diffuse = new();
        public Vector3 Specular = new();
        public Vector3 Emissive = new();
        public float Gloss = 0;
        public float Power = 0;

        public void Draw() {
            ImGui.ColorEdit3( "##Diffuse", ref Diffuse, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( "##Specular", ref Specular, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( "##Emissive", ref Emissive, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            using var disabled = ImRaii.Disabled();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( "##Gloss", ref Gloss, 0, 0, $"Gloss: %.1f", ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( "##Power", ref Gloss, 0, 0, $"Power: %.1f", ImGuiInputTextFlags.ReadOnly );
        }
    }
}
