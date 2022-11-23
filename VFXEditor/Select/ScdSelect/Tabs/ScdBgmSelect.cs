using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmSelect : SelectTab<XivBgm, XivBgm> {
        public ScdBgmSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.Bgm, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawPath( "Path", Loaded.Path, parentId, SelectResultType.GameMusic, Loaded.Name );
        }

        protected override string GetName( XivBgm item ) => item.Name;
    }
}