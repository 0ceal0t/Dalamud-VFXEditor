using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MtrlFormat.Data.Dye;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Parsing;
using VfxEditor.Parsing.HalfFloat;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public class MtrlColorRow : MtrlColorRowBase {
        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf Unknown1 = new( "Unknown 1", 1f );

        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf Unknown2 = new( "Unknown 2", 1f );

        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedHalf Unknown3 = new( "Unknown 3", 1f );

        public readonly ParsedHalf SheenRate = new( "Sheen Rate", 0.1f );
        public readonly ParsedHalf SheenTint = new( "Sheen Tint", 0.2f );
        public readonly ParsedHalf SheenAperature = new( "Sheen Aperature", 5f );
        public readonly ParsedHalf Unknown4 = new( "Unknown 4" );

        public readonly ParsedHalf Roughness = new( "Roughness", 0.5f );
        public readonly ParsedHalf Unknown5 = new( "Unknown 5" );
        public readonly ParsedHalf Metalness = new( "Metalness" );
        public readonly ParsedHalf Anisotropy = new( "Anisotropy" );

        public readonly ParsedHalf Unknown6 = new( "Unknown 6" );
        public readonly ParsedHalf SphereMask = new( "Sphere Mask" );
        public readonly ParsedHalf Unknown7 = new( "Unknown 7" );
        public readonly ParsedHalf Unknown8 = new( "Unknown 8" );

        public readonly ParsedShort Shader = new( "Shader" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf TileAlpha = new( "Tile Alpha" );
        public readonly ParsedSphereMaterial Sphere = new( "Sphere" );

        public readonly ParsedHalf4 TileMatrix = new( "Tile Matrix" );

        public readonly MtrlDyeRow DyeRow = new();

        public MtrlColorRow( MtrlTableBase table ) : base( table ) { }

        public override void Read( BinaryReader reader ) {
            Diffuse.Read( reader );
            Unknown1.Read( reader );
            Specular.Read( reader );
            Unknown2.Read( reader );
            Emissive.Read( reader );
            Unknown3.Read( reader );
            SheenRate.Read( reader );
            SheenTint.Read( reader );
            SheenAperature.Read( reader );
            Unknown4.Read( reader );
            Roughness.Read( reader );
            Unknown5.Read( reader );
            Metalness.Read( reader );
            Anisotropy.Read( reader );
            Unknown6.Read( reader );
            SphereMask.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            Shader.Read( reader );
            TileMaterial.Read( reader );
            TileAlpha.Read( reader );
            Sphere.Read( reader );
            TileMatrix.Read( reader );
        }

        public override void ReadDye( BinaryReader reader ) => DyeRow.Read( reader );

        public override void Write( BinaryWriter writer ) {
            Diffuse.Write( writer );
            Unknown1.Write( writer );
            Specular.Write( writer );
            Unknown2.Write( writer );
            Emissive.Write( writer );
            Unknown3.Write( writer );
            SheenRate.Write( writer );
            SheenTint.Write( writer );
            SheenAperature.Write( writer );
            Unknown4.Write( writer );
            Roughness.Write( writer );
            Unknown5.Write( writer );
            Metalness.Write( writer );
            Anisotropy.Write( writer );
            Unknown6.Write( writer );
            SphereMask.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            Shader.Write( writer );
            TileMaterial.Write( writer );
            TileAlpha.Write( writer );
            Sphere.Write( writer );
            TileMatrix.Write( writer );
        }

        public override void WriteDye( BinaryWriter writer ) => DyeRow.Write( writer );

        protected override void DrawDye() => DyeRow.Draw();

        protected override void DrawLeftItemColors() {
            Diffuse.DrawPreview();
            ImGui.SameLine();
            Specular.DrawPreview();
            ImGui.SameLine();
            Emissive.DrawPreview();
            ImGui.SameLine();
        }

        protected override void DrawTabs() {
            using( var tab = ImRaii.TabItem( "Color" ) ) {
                if( tab ) {
                    Shader.Draw();
                    Diffuse.Draw();
                    Specular.Draw();
                    Emissive.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Texture" ) ) {
                if( tab ) {
                    SheenRate.Draw();
                    SheenTint.Draw();
                    SheenAperature.Draw();
                    Metalness.Draw();
                    Roughness.Draw();
                    Anisotropy.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Tiling and Sphere" ) ) {
                if( tab ) {
                    Sphere.Draw();
                    SphereMask.Draw();
                    TileAlpha.Draw();
                    TileMaterial.Draw();
                    TileMatrix.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Unknown" ) ) {
                if( tab ) {
                    Unknown1.Draw();
                    Unknown2.Draw();
                    Unknown3.Draw();
                    Unknown4.Draw();
                    Unknown5.Draw();
                    Unknown6.Draw();
                    Unknown7.Draw();
                    Unknown8.Draw();
                }
            }
        }
    }
}
