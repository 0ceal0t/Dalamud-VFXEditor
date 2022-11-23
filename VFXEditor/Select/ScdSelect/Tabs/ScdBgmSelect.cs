using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdBgmSelect : SelectTab<XivBgm> {
        public ScdBgmSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.Bgm, dialog ) { }

        protected override void DrawSelected( string parentId ) {
            DrawPath( "Path", Selected.Path, parentId, SelectResultType.GameMusic, Selected.Name );
        }

        protected override string GetName( XivBgm item ) => item.Name;
    }
}