using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Numerics;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public struct MtrlResolvedRow {
        public Vector3 Diffuse;
        public Vector3 Specular;
        public Vector3 Emissive;
        public float Exposure;

        public float Roughness;
        public float Metalness;
        public float Anisotropy;

        public float SheenRate;
        public float SheenTint;
        public float SheenAperture;

        public int SphereIndex;
        public float SphereMask;

        public int ShaderId;
        public int TileIndex;
        public float TileAlpha;
        public Vector4 TileTransform;

        public bool Dyed;

        public static float RoughnessFromGloss( float gloss ) => MathF.Sqrt( 2f / ( Math.Max( gloss, 0f ) * 10f + 2f ) );

        public readonly void Draw() {
            var diffuse = Diffuse;
            var specular = Specular;
            var emissive = Emissive;
            var flags = ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoPicker;

            ImGui.ColorEdit3( "##ResultDiffuse", ref diffuse, flags );
            ImGui.SameLine();
            ImGui.ColorEdit3( "##ResultSpecular", ref specular, flags );
            ImGui.SameLine();
            ImGui.ColorEdit3( "##ResultEmissive", ref emissive, flags );

            using var disabled = ImRaii.Disabled();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 125 );
            var roughness = Roughness;
            ImGui.InputFloat( "##Roughness", ref roughness, 0, 0, $"Roughness: %.1f", ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.SetNextItemWidth( 125 );
            var metalness = Metalness;
            ImGui.InputFloat( "##Metalness", ref metalness, 0, 0, $"Metalness: %.1f", ImGuiInputTextFlags.ReadOnly );
        }
    }
}