using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.PAP {
    public class PAPActionSelect : PAPSelectTab<XivActionPap, XivActionPapSelected> {
        private ImGuiScene.TextureWrap Icon;

        public PAPActionSelect( string parentId, string tabId, PAPSelectDialog dialog ) :
            base( parentId, tabId, SheetManager.ActionPap, dialog ) {
        }

        protected override bool CheckMatch( XivActionPap item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionPapSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.ActionPap.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPapDict( loadedItem.StartAnimations, "Start", loadedItem.ActionPap.Name );

            DrawPapDict( loadedItem.EndAnimations, "End", loadedItem.ActionPap.Name );

            DrawPapDict( loadedItem.HitAnimations, "Hit", loadedItem.ActionPap.Name );
        }

        protected override string UniqueRowTitle( XivActionPap item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
