using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Data.Command;
using VfxEditor.DirectX;
using VfxEditor.Formats.MtrlFormat.Data.Table;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Data.Color {
    public abstract class MtrlColorRowBase : IUiItem {
        public readonly int RenderId = RenderInstance.NewId;
        public readonly MtrlTableBase Table;

        public MtrlColorRowBase( MtrlTableBase table ) {
            Table = table;
        }

        public abstract void Read( BinaryReader reader );

        public abstract void ReadDye( BinaryReader reader );

        public abstract void Write( BinaryWriter writer );

        public abstract void WriteDye( BinaryWriter writer );

        protected abstract void DrawTabs();

        protected abstract void DrawDye();

        protected virtual void DrawPreview( bool edited ) { }

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

        public virtual bool DrawLeftItem( int idx, bool selected ) {
            var ret = false;
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                DrawLeftItemColors();
            }
            if( ImGui.Selectable( $"#{idx}", selected ) ) ret = true;
            return ret;
        }

        protected abstract void DrawLeftItemColors();
    }
}
