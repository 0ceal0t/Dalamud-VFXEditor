using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Data.Color;
using VfxEditor.Utils;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public enum ColorTableSize : int {
        Legacy = 16 * 32,
        Extended = 32 * 64
    }

    public enum DyeTableSize : int {
        Legacy = 16 * 2,
        Extended = 32 * 4
    }

    public abstract class MtrlTableBase {
        public readonly MtrlFile File;

        public MtrlTableBase( MtrlFile file ) {
            File = file;
        }

        public abstract void Write( BinaryWriter writer );

        public abstract void Draw();
    }

    public abstract class MtrlTableBase<T> : MtrlTableBase where T : MtrlColorRowBase {
        public readonly List<T> Rows = [];
        public readonly MtrlColorRowSplitView<T> RowView;

        protected MtrlStain Stain;

        protected MtrlTableBase( MtrlFile file ) : base( file ) {
            for( var i = 0; i < GetRowCount(); i++ ) Rows.Add( GetNew() );
            RowView = new( Rows );
        }

        public MtrlTableBase( MtrlFile file, BinaryReader reader, long dataEnd ) : this( file ) {
            foreach( var row in Rows ) row.Read( reader );

            // Read dye rows
            if( !file.DyeTableEnabled || ( int )( dataEnd - reader.BaseStream.Position ) < ( int )GetDyeTableSize() ) return;
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

        protected abstract int GetRowCount();

        protected abstract T GetNew();

        protected abstract DyeTableSize GetDyeTableSize();

        private void DrawDyeCombo() {
            var v = Stain == null ? new( 0 ) : Stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            ImGui.SetNextItemWidth( 200f );
            using var combo = ImRaii.Combo( "Preview Dye", Stain == null ? "[NONE]" : Stain.Name );
            if( !combo ) return;

            DrawDyeComboRow( null, 0 );
            foreach( var (item, idx) in Plugin.MtrlManager.Stains.WithIndex() ) DrawDyeComboRow( item, idx + 1 );
        }

        private void DrawDyeComboRow( MtrlStain stain, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var v = stain == null ? new( 0 ) : stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            if( ImGui.Selectable( stain == null ? "[NONE]" : stain.Name, Stain == stain ) ) {
                Stain = stain;
                //foreach( var item in Rows ) item.SetPreviewStain( stain );
                //RowView.GetSelected()?.UpdateRender();
            }

            if( Stain == stain ) ImGui.SetItemDefaultFocus();
        }
    }
}
