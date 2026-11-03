using Dalamud.Bindings.ImGui;

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
            ImGui.SameLine();
            ImGui.TextDisabled( $"Exposure: {Exposure:0.##}  Metalness: {Metalness:0.##}  Roughness: {Roughness:0.##}" );
            if( !ImGui.IsItemHovered() ) return;

            ImGui.BeginTooltip();
            ImGui.Text( $"Sheen Rate: {SheenRate:0.###}" );
            ImGui.Text( $"Sheen Tint Rate: {SheenTintRate:0.###}" );
            ImGui.Text( $"Sheen Aperture: {SheenAperture:0.###}" );
            ImGui.Text( $"Anisotropy: {Anisotropy:0.###}" );
            ImGui.Text( $"Sphere Map: {SphereMapIndex}" );
            ImGui.Text( $"Sphere Map Mask: {SphereMapMask:0.###}" );
            ImGui.EndTooltip();
        }
    }
}