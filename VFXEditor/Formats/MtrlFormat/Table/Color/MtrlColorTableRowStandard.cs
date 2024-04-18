using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Formats.MtrlFormat.Table.Dye;
using VfxEditor.Parsing.HalfFloat;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public class MtrlColorTableRowStandard : MtrlColorTableRow {
        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf MaterialRepeatX = new( "Material Repeat X", 16f );
        public readonly ParsedHalf2 MaterialSkew = new( "Material Skew" );
        public readonly ParsedHalf MaterialRepeatY = new( "Material Repeat Y", 16f );

        public MtrlDyeTableRowStandard DyeRow { get; protected set; }

        public MtrlColorTableRowStandard( MtrlFile file ) : base( file ) { }

        public MtrlColorTableRowStandard( MtrlFile file, BinaryReader reader ) : base( file ) {
            Diffuse.Read( reader );
            SpecularStrength.Read( reader );
            Specular.Read( reader );
            GlossStrength.Read( reader );
            Emissive.Read( reader );
            TileMaterial.Read( reader );
            MaterialRepeatX.Read( reader );
            MaterialSkew.Read( reader );
            MaterialRepeatY.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Diffuse.Write( writer );
            SpecularStrength.Write( writer );
            Specular.Write( writer );
            GlossStrength.Write( writer );
            Emissive.Write( writer );
            TileMaterial.Write( writer );
            MaterialRepeatX.Write( writer );
            MaterialSkew.Write( writer );
            MaterialRepeatY.Write( writer );
        }

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
                    MaterialRepeatX.Draw();
                    MaterialRepeatY.Draw();
                    MaterialSkew.Draw();
                }
            }
        }

        public override StmDyeData GetStainTemplate() => Stain == null ? null : Plugin.MtrlManager.StmFile.GetDye( DyeRow.Template.Value, ( int )Stain.Id );

        public override void RefreshPreview() => Plugin.DirectXManager.MaterialPreview.LoadColorRow( this );

        public override void InitDye() { DyeRow = new(); }
        public override void InitDye( BinaryReader reader ) { DyeRow = new( reader ); }
        public override void WriteDye( BinaryWriter writer ) => DyeRow.Write( writer );
        protected override void DrawDyeRow() => DyeRow.Draw();
    }
}
