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
    public class MtrlColorTableRow : IUiItem {
        public readonly int RenderId = Renderer.NewId;
        public readonly MtrlTables Tables;
        public readonly MtrlDyeTableRow DyeRow;

        public MtrlStain Stain { get; private set; }
        public StmDyeData StainTemplate { get; private set; }

        public readonly ParsedHalf3Color Diffuse = new( "Diffuse", Vector3.One );
        public readonly ParsedHalf SpecularStrength = new( "Specular Strength", 1f );
        public readonly ParsedHalf3Color Specular = new( "Specular", Vector3.One );
        public readonly ParsedHalf GlossStrength = new( "Gloss Strength", 20f );
        public readonly ParsedHalf3Color Emissive = new( "Emissive" );
        public readonly ParsedTileMaterial TileMaterial = new( "Tile Material" );
        public readonly ParsedHalf TileRepeatX = new( "Repeat X", 16f );
        public readonly ParsedHalf2 TileSkew = new( "Skew" );
        public readonly ParsedHalf TileRepeatY = new( "Repeat Y", 16f );

        public readonly ParsedHalf TileNormalScale = new( "Normal Scale" );

        public MtrlColorTableRow( MtrlTables tables ) {
            Tables = tables;
            DyeRow = new( tables );
        }

        // ====== READ+WRITE =========

        public void Read( BinaryReader reader ) {
            if( !Tables.Extended ) {
                Diffuse.Read( reader );
                SpecularStrength.Read( reader );
                Specular.Read( reader );
                GlossStrength.Read( reader );
                Emissive.Read( reader );
                TileMaterial.Read( reader );
                TileRepeatX.Read( reader );
                TileSkew.Read( reader );
                TileRepeatY.Read( reader );

                return;
            }

            Diffuse.Read( reader );
            GlossStrength.Read( reader );
            Specular.Read( reader );
            SpecularStrength.Read( reader );

            for( var i = 0; i < 5; i++ ) {
                Dalamud.Log( $"{i} >> {reader.ReadHalf()} {reader.ReadHalf()} {reader.ReadHalf()} {reader.ReadHalf()}" );
            }

            TileRepeatX.Read( reader );
            TileSkew.Read( reader );
            TileRepeatY.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            if( !Tables.Extended ) {
                Diffuse.Write( writer );
                SpecularStrength.Write( writer );
                Specular.Write( writer );
                GlossStrength.Write( writer );
                Emissive.Write( writer );
                TileMaterial.Write( writer );
                TileRepeatX.Write( writer );
                TileSkew.Write( writer );
                TileRepeatY.Write( writer );

                return;
            }

            Diffuse.Write( writer );
            GlossStrength.Write( writer );
            Specular.Write( writer );
            SpecularStrength.Write( writer );
            FileUtils.Pad( writer, 8 * 5 ); // temp
            TileRepeatX.Write( writer );
            TileSkew.Write( writer );
            TileRepeatY.Write( writer );
        }

        // ====== DRAWING ===========

        private void DrawTabs() {
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
