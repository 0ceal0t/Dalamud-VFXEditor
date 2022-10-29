using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxEmoteSelect : VfxSelectTab<XivEmote, XivEmoteSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxEmoteSelect( string parentId, string tabId, VfxSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.Emotes, dialog ) {
        }

        protected override bool CheckMatch( XivEmote item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivEmoteSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Emote.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "VFX", loadedItem.VfxPaths, Id, Dialog, SelectResultType.GameEmote, "EMOTE", loadedItem.Emote.Name, spawn: true );
        }

        protected override string UniqueRowTitle( XivEmote item ) {
            return item.Name + Id + item.RowId;
        }
    }
}