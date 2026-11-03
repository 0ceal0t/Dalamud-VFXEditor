using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MtrlFormat.Data.Dye;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Parsing.HalfFloat;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public class MtrlColorRowLegacy : MtrlColorRowBase {
        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf TileRepeatX = new( "Repeat X", 16f );
        public readonly ParsedHalf2 TileSkew = new( "Skew" );
        public readonly ParsedHalf TileRepeatY = new( "Repeat Y", 16f );

        public readonly MtrlDyeRowLegacy DyeRow = new();

        public MtrlColorRowLegacy( MtrlFile file, MtrlTableBase table ) : base( file, table ) { }

        public override void Read( BinaryReader reader ) {
            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileMaterial.Read( reader );
            TileRepeatX.Read( reader );
            TileSkew.Read( reader );
            TileRepeatY.Read( reader );
        }

        public override void ReadDye( BinaryReader reader ) => DyeRow.Read( reader );

        public override void Write( BinaryWriter writer ) {
            Diffuse.Write( writer );
            SpecularStrength.Write( writer );
            Specular.Write( writer );
            GlossStrength.Write( writer );
            Emissive.Write( writer );
            TileMaterial.Write( writer );
            TileRepeatX.Write( writer );
            TileSkew.Write( writer );
            TileRepeatY.Write( writer );
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
                    Diffuse.Draw();
                    SpecularStrength.Draw();
                    Specular.Draw();
                    GlossStrength.Draw();
                    Emissive.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Tiling" ) ) {
                if( tab ) {
                    TileMaterial.Draw();
                    TileRepeatX.Draw();
                    TileRepeatY.Draw();
                    TileSkew.Draw();
                }
            }
        }

        // ===== PREVIEW =========

        protected override int DyeChannel => 0;

        public override StmDyeDataBase GetStainTemplate() => Stain == null ? null : Plugin.MtrlManager.StmFileLegacy.GetDye( DyeRow.Template.Value, ( int )Stain.Id );

        public override MtrlResolvedRow Resolve( bool applyDye ) {
            var dye = applyDye ? StainTemplate as StmDyeDataLegacy : null;
            var flags = DyeRow.Flags.Value;
            bool Dyed( DyeRowFlags_Legacy flag ) => dye != null && flags.HasFlag( flag );

            var specular = Dyed( DyeRowFlags_Legacy.Specular ) ? dye.Specular : Specular.Value;
            var specularStrength = Dyed( DyeRowFlags_Legacy.Specular_Strength ) ? dye.Power : SpecularStrength.Value;
            var gloss = Dyed( DyeRowFlags_Legacy.Gloss ) ? dye.Gloss : GlossStrength.Value;

            return new() {
                Diffuse = Dyed( DyeRowFlags_Legacy.Diffuse ) ? dye.Diffuse : Diffuse.Value,
                Specular = specular * specularStrength,
                Emissive = Dyed( DyeRowFlags_Legacy.Emissive ) ? dye.Emissive : Emissive.Value,
                Exposure = 1f,
                Roughness = MtrlResolvedRow.RoughnessFromGloss( gloss ),
                TileIndex = TileMaterial.Value,
                TileAlpha = 1f,
                TileTransform = new( TileRepeatX.Value, TileSkew.Value.X, TileSkew.Value.Y, TileRepeatY.Value ),
                Dyed = dye != null && flags != 0,
            };
        }

        public override void UpdateRender() {
            StainTemplate = GetStainTemplate();
            Plugin.DirectXManager.MaterialRenderer.SetColorRow( RenderId, File.Instance, Resolve( true ) );
        }

        protected override void DrawRender() => Plugin.DirectXManager.MaterialRenderer.DrawTexture( RenderId, File.Instance, UpdateRender, Plugin.Configuration.DrawDirectXMaterial );
    }
}