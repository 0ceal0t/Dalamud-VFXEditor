using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Data;
using VfxEditor.Data.Command;
using VfxEditor.DirectX;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Formats.MtrlFormat.Stm;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public abstract class MtrlColorRowBase : IUiItem {
        public readonly int RenderId = RenderInstance.NewId;
        public readonly MtrlTableBase Table;
        public readonly MtrlFile File;

        public MtrlStain Stain { get; protected set; }
        public StmDyeData StainTemplate { get; protected set; }

        public MtrlColorRowBase( MtrlFile file, MtrlTableBase table ) {
            Table = table;
            File = file;
        }

        public abstract void Read( BinaryReader reader );

        public abstract void ReadDye( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );

        public abstract void WriteDye( BinaryWriter writer );

        protected abstract void DrawTabs();

        protected abstract void DrawDye();

        public void Draw() {
            using var editing = new Edited();

            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            DrawTabs();

            using( var disabled = ImRaii.Disabled( !Table.File.DyeTableEnabled ) )
            using( var tab = ImRaii.TabItem( "Dye" ) ) {
                if( tab ) DrawDye();
            }

            DrawPreview( editing.IsEdited );
        }

        protected abstract void DrawLeftItemColors();

        public bool DrawLeftItem( int idx, bool selected ) {
            var ret = false;
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                DrawLeftItemColors();
            }
            if( ImGui.Selectable( $"#{idx}", selected ) ) ret = true;


            if( StainTemplate != null ) {
                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                ImGui.TextDisabled( FontAwesomeIcon.PaintBrush.ToIconString() );
            }

            return ret;
        }

        protected void DrawPreview( bool edited ) {
            if( Stain != null ) {
                using var child = ImRaii.Child( "Child", new( -1, ImGui.GetFrameHeight() + ImGui.GetStyle().WindowPadding.Y * 2 ), true );
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                if( StainTemplate == null ) ImGui.TextDisabled( "[NO DYE VALUE]" );
                else StainTemplate.Draw();
            }

            if( edited ) UpdateRender();

            // TODO
            //Plugin.DirectXManager.MaterialRenderer.DrawTexture( RenderId, File.Instance, UpdateRender, ConfigurationWindow.DrawDirectXMaterial );
        }

        public void SetPreviewStain( MtrlStain stain ) {
            Stain = stain;
            StainTemplate = GetStainTemplate();
        }

        public abstract StmDyeData GetStainTemplate();

        public abstract void UpdateRender();
    }
}
