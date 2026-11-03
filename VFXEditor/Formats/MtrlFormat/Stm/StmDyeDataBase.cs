using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public abstract class StmDyeDataBase {
        public Vector3 Diffuse = new();
        public Vector3 Specular = new();
        public Vector3 Emissive = new();

        public void Draw() {
            ImGui.ColorEdit3( "##Diffuse", ref Diffuse, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( "##Specular", ref Specular, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( "##Emissive", ref Emissive, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            DrawScalars();
        }

        protected abstract void DrawScalars();
    }
}