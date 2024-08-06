using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Data.Color;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public class MtrlTableLegacy : MtrlTableBase {
        public readonly List<MtrlColorRowLegacy> Rows = [];
        public readonly MtrlColorRowSplitView<MtrlColorRowLegacy> RowView;

        private MtrlStain Stain;

        public MtrlTableLegacy( MtrlFile file ) : base( file ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( this ) );
            RowView = new( Rows );
        }

        public MtrlTableLegacy( MtrlFile file, BinaryReader reader, long dataEnd ) : this( file ) {
            foreach( var row in Rows ) row.Read( reader );

            // Read dye rows
            if( !file.DyeTableEnabled || ( int )( dataEnd - reader.BaseStream.Position ) < ( int )DyeTableSize.Legacy ) return;
            foreach( var row in Rows ) row.ReadDye( reader );
        }

        public override void Write( BinaryWriter writer ) {
            if( File.ColorTableEnabled ) foreach( var row in Rows ) row.Write( writer );
            if( File.DyeTableEnabled ) foreach( var row in Rows ) row.WriteDye( writer );
        }

        public override void Draw() {
            DrawDyeCombo();
            ImGui.Separator();
            RowView.Draw();
        }

        // ======= STAIN =========

        private void DrawDyeCombo() {
            var v = Stain == null ? new( 0 ) : Stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            ImGui.SetNextItemWidth( 200f );
            using var combo = ImRaii.Combo( "Preview Dye", Stain == null ? "[NONE]" : Stain.Name );
            if( !combo ) return;

            DrawDyeComboRow( null, 0 );
            foreach( var (item, idx) in Plugin.MtrlManager.LegacyStains.WithIndex() ) DrawDyeComboRow( item, idx + 1 );
        }

        private void DrawDyeComboRow( MtrlStain stain, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var v = stain == null ? new( 0 ) : stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            if( ImGui.Selectable( stain == null ? "[NONE]" : stain.Name, Stain == stain ) ) {
                Stain = stain;
                foreach( var item in Rows ) item.SetPreviewStain( stain );
                RowView.GetSelected()?.RefreshPreview();
            }

            if( Stain == stain ) ImGui.SetItemDefaultFocus();
        }
    }
}
