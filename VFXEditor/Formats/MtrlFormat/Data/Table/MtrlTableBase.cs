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

        public abstract bool IsValidDimensionLogs( int logs );

        public abstract int DefaultDimensionLogs { get; }

        public virtual MtrlStain GetPreviewStain( int channel ) => null;

        public abstract void Write( BinaryWriter writer );

        public abstract void Draw();
    }

    public abstract class MtrlTableBase<T> : MtrlTableBase where T : MtrlColorRowBase {
        public readonly List<T> Rows = [];
        public readonly MtrlColorRowSplitView<T> RowView;

        protected readonly MtrlStain[] Stains;

        protected MtrlTableBase( MtrlFile file ) : base( file ) {
            Stains = new MtrlStain[GetDyeChannelCount()];
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
            if( !File.ColorTableEnabled ) return;
            foreach( var row in Rows ) row.Write( writer );
            if( File.DyeTableEnabled ) foreach( var row in Rows ) row.WriteDye( writer );
        }

        public override MtrlStain GetPreviewStain( int channel ) => channel >= 0 && channel < Stains.Length ? Stains[channel] : null;

        public override void Draw() {
            for( var i = 0; i < Stains.Length; i++ ) {
                using var _ = ImRaii.PushId( i );
                DrawDyeCombo( i );
            }
            ImGui.Separator();
            RowView.Draw();
        }

        protected abstract int GetRowCount();

        protected abstract T GetNew();

        protected abstract DyeTableSize GetDyeTableSize();

        protected abstract int GetDyeChannelCount();

        private void DrawDyeCombo( int channel ) {
            var stain = Stains[channel];
            var v = stain == null ? new( 0 ) : stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            ImGui.SetNextItemWidth( 200f );
            using var combo = ImRaii.Combo( Stains.Length > 1 ? $"Preview Dye {channel + 1}" : "Preview Dye", stain == null ? "[NONE]" : stain.Name );
            if( !combo ) return;

            DrawDyeComboRow( channel, null, 0 );
            foreach( var (item, idx) in Plugin.MtrlManager.Stains.WithIndex() ) DrawDyeComboRow( channel, item, idx + 1 );
        }

        private void DrawDyeComboRow( int channel, MtrlStain stain, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var v = stain == null ? new( 0 ) : stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            if( ImGui.Selectable( stain == null ? "[NONE]" : stain.Name, Stains[channel] == stain ) ) {
                Stains[channel] = stain;
                foreach( var item in Rows ) item.RefreshStainTemplate();
                RowView.GetSelected()?.UpdateRender();
            }

            if( Stains[channel] == stain ) ImGui.SetItemDefaultFocus();
        }
    }
}