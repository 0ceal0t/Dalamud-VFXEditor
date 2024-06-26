using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.IO;
using System.Numerics;
using VfxEditor.Data.Command;
using VfxEditor.DirectX;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Formats.MtrlFormat.Table.Dye;
using VfxEditor.Parsing.HalfFloat;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public partial class MtrlColorTableRow : IUiItem {
        public readonly int RenderId = Renderer.NewId;
        public readonly MtrlTables Tables;
        public readonly MtrlDyeTableRow DyeRow;

        public MtrlStain Stain { get; private set; }
        public StmDyeData StainTemplate { get; private set; }

        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedHalf3Color UnknownColor = new( "Unknown Color", Vector3.One );

        public readonly ParsedHalf Unknown1 = new( "Unknown 1" );
        public readonly ParsedHalf Unknown2 = new( "Unknown 2" );
        public readonly ParsedHalf Unknown3 = new( "Unknown 3" );
        public readonly ParsedHalf Unknown4 = new( "Unknown 4" );
        public readonly ParsedHalf Unknown5 = new( "Unknown 5" );
        public readonly ParsedHalf Unknown6 = new( "Unknown 6" );

        public readonly ParsedHalf BlendingAnisotropy = new( "Blending Anisotrophy" );
        public readonly ParsedHalf SpmIndex = new( "SPM Index" ); // TODO
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf NormalScale = new( "Normal Scale" );

        public readonly ParsedHalf TileNormalScale = new( "Normal Scale" );
        public readonly ParsedHalf TileRepeatX = new( "Repeat X", 16f );
        public readonly ParsedHalf2 TileSkew = new( "Skew" );
        public readonly ParsedHalf TileRepeatY = new( "Repeat Y", 16f );

        public MtrlColorTableRow( MtrlTables tables ) {
            Tables = tables;
            DyeRow = new( tables );
        }

        // ====== READ+WRITE =========

        public void Read( BinaryReader reader ) {
            if( ReadLegacy( reader ) ) return;

            Diffuse.Read( reader );
            GlossStrength.Read( reader );
            Specular.Read( reader );
            SpecularStrength.Read( reader );

            UnknownColor.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            reader.ReadHalf(); // TODO
            Unknown5.Read( reader );
            reader.ReadHalf(); // TODO
            Unknown6.Read( reader );

            BlendingAnisotropy.Read( reader );
            for( var i = 0; i < 4; i++ ) reader.ReadHalf(); // TODO
            SpmIndex.Read( reader );
            TileMaterial.Read( reader );
            NormalScale.Read( reader );
            reader.ReadHalf(); // TODO

            TileRepeatX.Read( reader );
            TileSkew.Read( reader );
            TileRepeatY.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            if( WriteLegacy( writer ) ) return;

            Diffuse.Write( writer );
            GlossStrength.Write( writer );
            Specular.Write( writer );
            SpecularStrength.Write( writer );

            UnknownColor.Write( writer );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            FileUtils.Pad( writer, 2 ); // TODO
            Unknown5.Write( writer );
            FileUtils.Pad( writer, 2 ); // TODO
            Unknown6.Write( writer );

            BlendingAnisotropy.Write( writer );
            FileUtils.Pad( writer, 4 * 2 ); // TODO
            SpmIndex.Write( writer );
            TileMaterial.Write( writer );
            NormalScale.Write( writer );
            FileUtils.Pad( writer, 2 ); // TODO

            TileRepeatX.Write( writer );
            TileSkew.Write( writer );
            TileRepeatY.Write( writer );
        }

        // ====== DRAWING ===========

        private void DrawTabs() {
            if( DrawTabsLegacy() ) return;

            using( var tab = ImRaii.TabItem( "Color" ) ) {
                if( tab ) {
                    Diffuse.Draw();
                    SpecularStrength.Draw();
                    Specular.Draw();
                    GlossStrength.Draw();
                    Emissive.Draw();
                    UnknownColor.Draw();
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
                    BlendingAnisotropy.Draw();
                }
            }

            using( var tab = ImRaii.TabItem( "Tiling" ) ) {
                if( tab ) {
                    TileMaterial.Draw();
                    SpmIndex.Draw();
                    NormalScale.Draw();
                    TileRepeatX.Draw();
                    TileRepeatY.Draw();
                    TileSkew.Draw();
                }
            }
        }

        public void Draw() {
            using var editing = new Edited();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawTabs();

            using( var disabled = ImRaii.Disabled( !Tables.File.DyeTableEnabled ) )
            using( var tab = ImRaii.TabItem( "Dye" ) ) {
                if( tab ) DyeRow.Draw();
            }

            if( Stain != null ) {
                using var child = ImRaii.Child( "Child", new( -1, ImGui.GetFrameHeight() + ImGui.GetStyle().WindowPadding.Y * 2 ), true );
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                if( StainTemplate == null ) ImGui.TextDisabled( "[NO DYE VALUE]" );
                else StainTemplate.Draw();
            }

            if( Plugin.DirectXManager.MaterialPreview.CurrentRenderId != RenderId || editing.IsEdited ) {
                StainTemplate = GetStainTemplate();
                RefreshPreview();
            }
            Plugin.DirectXManager.MaterialPreview.DrawInline();
        }

        public void DrawLeftItem( int idx, ref MtrlColorTableRow selected ) {
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                DrawLeftItemColors();
            }
            if( ImGui.Selectable( $"#{idx}", this == selected ) ) selected = this;
            if( StainTemplate != null ) {
                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.TextDisabled( FontAwesomeIcon.PaintBrush.ToIconString() );
            }
        }

        private void DrawLeftItemColors() {
            Diffuse.DrawPreview();
            ImGui.SameLine();
            Specular.DrawPreview();
            ImGui.SameLine();
            Emissive.DrawPreview();
            ImGui.SameLine();
            if( !Tables.Legacy ) {
                UnknownColor.DrawPreview();
                ImGui.SameLine();
            }
        }

        // ===== PREVIEW =========

        public StmDyeData GetStainTemplate() => Stain == null ? null : Plugin.MtrlManager.StmFile.GetDye( DyeRow.Template.Value, ( int )Stain.Id );

        public void RefreshPreview() => Plugin.DirectXManager.MaterialPreview.LoadColorRow( this );

        public void SetPreviewStain( MtrlStain stain ) {
            Stain = stain;
            StainTemplate = GetStainTemplate();
        }
    }
}
