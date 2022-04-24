using ImGuiNET;
using VFXSelect.Select.Rows;

namespace VFXSelect.VFX {
    public class VFXActionSelect : VFXSelectTab<XivActionBase, XivActionSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VFXActionSelect( string parentId, string tabId, VFXSelectDialog dialog, bool nonPlayer = false ) :
            base( parentId, tabId, !nonPlayer ? SheetManager.Actions : SheetManager.NonPlayerActions, dialog ) {
        }

        protected override bool CheckMatch( XivActionBase item, string searchInput ) {
            return Matches( item.Name, searchInput );
        }

        protected override void OnSelect() {
            LoadIcon( Selected.Icon, ref Icon );
        }

        protected override void DrawSelected( XivActionSelected loadedItem ) {
            if( loadedItem == null ) { return; }
            ImGui.Text( loadedItem.Action.Name );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            DrawIcon( Icon );

            DrawPath( "Cast VFX Path", loadedItem.CastVfxPath, Id + "Cast", Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Action.Name + " Cast", spawn: true );

            if( loadedItem.SelfVfxExists ) {
                ImGui.Text( "TMB Path: " );
                ImGui.SameLine();
                DisplayPath( loadedItem.SelfTmbPath );
                Copy( loadedItem.SelfTmbPath, id: Id + "CopyTmb" );

                DrawPath( "VFX", loadedItem.SelfVfxPaths, Id, Dialog, SelectResultType.GameAction, "ACTION", loadedItem.Action.Name, spawn: true );
            }
        }

        protected override string UniqueRowTitle( XivActionBase item ) {
            return item.Name + "##" + item.RowId;
        }
    }
}
