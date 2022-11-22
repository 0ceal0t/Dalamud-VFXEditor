using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapActionSelect : PapSelectTab<XivActionPap, XivActionPapSelected> {
        private ImGuiScene.TextureWrap Icon;

        public PapActionSelect( string parentId, string tabId, PapSelectDialog dialog, bool nonPlayer = false ) :
            base( parentId, tabId, nonPlayer ? SheetManager.NonPlayerActionPap : SheetManager.ActionPap, dialog ) {
        }

        protected override bool CheckMatch( XivActionPap item, string searchInput ) => Matches( item.Name, searchInput );

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

        protected override string UniqueRowTitle( XivActionPap item ) => $"{item.Name}##{item.RowId}";
    }
}
