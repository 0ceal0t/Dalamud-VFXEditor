using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmSelect : ScdSelectTab<XivBgm, XivBgm> {
        public ScdBgmSelect( string parentId, string tabId, ScdSelectDialog dialog ) : base( parentId, tabId, SheetManager.Bgm, dialog ) { }

        protected override bool CheckMatch( XivBgm item, string searchInput ) => Matches( item.Name, searchInput );

        protected override void DrawSelected( XivBgm loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawPath( "Path", loadedItem.Path, Id, Dialog, SelectResultType.GameMusic, "MUSIC", loadedItem.Name );
        }

        protected override string UniqueRowTitle( XivBgm item ) => $"{item.Name}##{item.RowId}";
    }
}