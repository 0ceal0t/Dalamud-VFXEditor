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

        public MtrlStain Stain { get; private set; }
        public StmDyeData StainTemplate { get; private set; }

        public MtrlColorRowLegacy( MtrlTableBase table ) : base( table ) { }

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

        protected override void DrawPreview( bool edited ) {
            if( Stain != null ) {
                using var child = ImRaii.Child( "Child", new( -1, ImGui.GetFrameHeight() + ImGui.GetStyle().WindowPadding.Y * 2 ), true );
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                if( StainTemplate == null ) ImGui.TextDisabled( "[NO DYE VALUE]" );
                else StainTemplate.Draw();
            }

            if( Plugin.DirectXManager.MaterialPreviewLegacy.CurrentRenderId != RenderId || edited ) {
                StainTemplate = GetStainTemplate();
                RefreshPreview();
            }
            Plugin.DirectXManager.MaterialPreviewLegacy.DrawInline();
        }

        public StmDyeData GetStainTemplate() => Stain == null ? null : Plugin.MtrlManager.StmFile.GetDye( DyeRow.Template.Value, ( int )Stain.Id );

        public void RefreshPreview() => Plugin.DirectXManager.MaterialPreviewLegacy.LoadColorRow( this );

        public void SetPreviewStain( MtrlStain stain ) {
            Stain = stain;
            StainTemplate = GetStainTemplate();
        }

        public override bool DrawLeftItem( int idx, bool selected ) {
            var ret = base.DrawLeftItem( idx, selected );

            if( StainTemplate != null ) {
                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.TextDisabled( FontAwesomeIcon.PaintBrush.ToIconString() );
            }

            return ret;
        }
    }
}
