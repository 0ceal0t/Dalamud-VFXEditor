using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.IO;
using VfxEditor.Data.Command;
using VfxEditor.DirectX;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Table.Color {
    public abstract class MtrlColorTableRow : IUiItem {
        public readonly int RenderId = Renderer.NewId;

        protected readonly MtrlFile File;

        public MtrlStain Stain { get; protected set; }
        public StmDyeData StainTemplate { get; protected set; }

        public MtrlColorTableRow( MtrlFile file ) {
            File = file;
        }

        public abstract void InitDye();
        public abstract void InitDye( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );
        public abstract void WriteDye( BinaryWriter writer );

        protected abstract void DrawTabs();
        protected abstract void DrawDyeRow();

        public void Draw() {
            using var editing = new Edited();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawTabs();

            using( var disabled = ImRaii.Disabled( !File.DyeTableEnabled ) )
            using( var tab = ImRaii.TabItem( "Dye" ) ) {
                if( tab ) DrawDyeRow();
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

            if( ImGui.Selectable( $"Row {idx}", this == selected ) ) selected = this;
            if( StainTemplate != null ) {
                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.TextDisabled( FontAwesomeIcon.PaintBrush.ToIconString() );
            }
        }

        protected abstract void DrawLeftItemColors();

        public abstract StmDyeData GetStainTemplate();

        public abstract void RefreshPreview();

        public void SetPreviewStain( MtrlStain stain ) {
            Stain = stain;
            StainTemplate = GetStainTemplate();
        }
    }
}
