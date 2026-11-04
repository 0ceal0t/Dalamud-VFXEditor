using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Formats.MtrlFormat.Data.Dye;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Parsing;
using VfxEditor.Parsing.HalfFloat;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public class MtrlColorRow : MtrlColorRowBase {
        // Maybe something like https://github.com/blender/blender/blob/main/source/blender/gpu/shaders/material/gpu_shader_material_principled.glsl ?

        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf Unknown1 = new( "Unknown 1", 1f );

        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf Unknown2 = new( "Unknown 2", 1f );

        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedHalf Exposure = new( "Exposure", 1f );

        public readonly ParsedHalf SheenRate = new( "Sheen Rate", 0.1f );
        public readonly ParsedHalf SheenTint = new( "Sheen Tint", 0.2f );
        public readonly ParsedHalf SheenAperture = new( "Sheen Aperture", 5f );
        public readonly ParsedHalf Unknown4 = new( "Unknown 4" );

        public readonly ParsedHalf Roughness = new( "Roughness", 0.5f );
        public readonly ParsedHalf Unknown5 = new( "Unknown 5" );
        public readonly ParsedHalf Metalness = new( "Metalness" );
        public readonly ParsedHalf Anisotropy = new( "Anisotropy" );

        public readonly ParsedHalf Unknown6 = new( "Unknown 6" );
        public readonly ParsedHalf SphereMask = new( "Sphere Mask" );
        public readonly ParsedHalf Unknown7 = new( "Unknown 7" );
        public readonly ParsedHalf Unknown8 = new( "Unknown 8" );

        public readonly ParsedHalfShort Shader = new( "Shader" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf TileAlpha = new( "Tile Alpha" );
        public readonly ParsedSphereMaterial Sphere = new( "Sphere" );

        public readonly ParsedHalf4 TileMatrix = new( "Tile Matrix" );

        public readonly MtrlDyeRow DyeRow = new();

        public MtrlColorRow( MtrlFile file, MtrlTableBase table ) : base( file, table ) { }

        public override void Read( BinaryReader reader ) {
            Diffuse.Read( reader );
            Unknown1.Read( reader );
            Specular.Read( reader );
            Unknown2.Read( reader );
            Emissive.Read( reader );
            Exposure.Read( reader );
            SheenRate.Read( reader );
            SheenTint.Read( reader );
            SheenAperture.Read( reader );
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
            Exposure.Write( writer );
            SheenRate.Write( writer );
            SheenTint.Write( writer );
            SheenAperture.Write( writer );
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
                    Exposure.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Texture" ) ) {
                if( tab ) {
                    SheenRate.Draw();
                    SheenTint.Draw();
                    SheenAperture.Draw();
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
                    Unknown4.Draw();
                    Unknown5.Draw();
                    Unknown6.Draw();
                    Unknown7.Draw();
                    Unknown8.Draw();
                }
            }
        }

        // ===== PREVIEW =========

        protected override int DyeChannel => DyeRow.Channel.Value;

        // Templates missing from the Dawntrail file fall back to the legacy one, same as Penumbra
        public override StmDyeDataBase GetStainTemplate() {
            if( Stain == null ) return null;
            var template = DyeRow.Template.Value;
            var stainId = ( int )Stain.Id;
            return ( StmDyeDataBase )Plugin.MtrlManager.StmFile.GetDye( template, stainId ) ?? Plugin.MtrlManager.StmFileLegacy.GetDye( template, stainId );
        }

        public override MtrlResolvedRow Resolve( bool applyDye ) {
            var dye = applyDye ? StainTemplate as StmDyeData : null;
            var flags = DyeRow.Flags.Value;
            bool Dyed( DyeRowFlags flag ) => dye != null && flags.HasFlag( flag );

            return new() {
                Diffuse = Dyed( DyeRowFlags.Diffuse ) ? dye.Diffuse : Diffuse.Value,
                // Same as Penumbra, a black specular in the template means it isn't dyed
                Specular = Dyed( DyeRowFlags.Specular ) && dye.Specular != Vector3.Zero ? dye.Specular : Specular.Value,
                Emissive = Dyed( DyeRowFlags.Emissive ) ? dye.Emissive : Emissive.Value,
                Exposure = Dyed( DyeRowFlags.Exposure ) ? dye.Exposure : Exposure.Value,
                Roughness = Dyed( DyeRowFlags.Roughness ) ? dye.Roughness : Roughness.Value,
                Metalness = Dyed( DyeRowFlags.Metalness ) ? dye.Metalness : Metalness.Value,
                Anisotropy = Dyed( DyeRowFlags.Anisotropy ) ? dye.Anisotropy : Anisotropy.Value,
                SheenRate = Dyed( DyeRowFlags.Sheen_Rate ) ? dye.SheenRate : SheenRate.Value,
                SheenTint = Dyed( DyeRowFlags.Sheen_Tint_Rate ) ? dye.SheenTintRate : SheenTint.Value,
                SheenAperture = Dyed( DyeRowFlags.Sheen_Aperture ) ? dye.SheenAperture : SheenAperture.Value,
                SphereIndex = Dyed( DyeRowFlags.Sphere_Map_Index ) ? dye.SphereMapIndex : Sphere.Value,
                SphereMask = Dyed( DyeRowFlags.Sphere_Map_Mask ) ? dye.SphereMapMask : SphereMask.Value,
                ShaderId = Shader.Value,
                TileIndex = TileMaterial.Value,
                TileAlpha = TileAlpha.Value,
                TileTransform = TileMatrix.Value,
                Dyed = dye != null && flags != 0,
            };
        }

        private MtrlResolvedRow ResolvePreview() {
            var resolved = Resolve( true );
            if( Table is not MtrlTable table || table.PairBlend <= 0f ) return resolved;

            var idx = table.Rows.IndexOf( this );
            var pairIdx = idx ^ 1;
            if( idx < 0 || pairIdx >= table.Rows.Count ) return resolved;

            var pair = table.Rows[pairIdx];
            pair.RefreshStainTemplate();
            return MtrlResolvedRow.Lerp( resolved, pair.Resolve( true ), table.PairBlend );
        }

        public override void UpdateRender() {
            StainTemplate = GetStainTemplate();
            Plugin.DirectXManager.MaterialRenderer.SetColorRow( RenderId, File.Instance, ResolvePreview(), false );
        }

        protected override void DrawRender() => Plugin.DirectXManager.MaterialRenderer.DrawTexture( RenderId, File.Instance, UpdateRender, DrawRenderPopup );

        private void DrawRenderPopup() {
            Plugin.Configuration.DrawDirectXMaterial();
            if( Table is not MtrlTable table ) return;

            ImGui.Separator();
            var idx = table.Rows.IndexOf( this );
            if( ImGui.SliderFloat( $"Blend with #{idx ^ 1}", ref table.PairBlend, 0f, 1f ) ) UpdateRender();
        }
    }
}