using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdMountSelect : SelectTab<XivMount, XivMountSelected> {
        private ImGuiScene.TextureWrap Icon;

        public ScdMountSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.Mounts, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "BGM Path", Loaded.Mount.Bgm, parentId, SelectResultType.GameNpc, Loaded.Mount.Name );
        }

        protected override string GetName( XivMount item ) => item.Name;
    }
}