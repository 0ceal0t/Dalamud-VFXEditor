using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbCommonSelect : SelectTab<XivCommon, XivCommon> {
        private ImGuiScene.TextureWrap Icon;

        public TmbCommonSelect( string tabId, TmbSelectDialog dialog ) : base( tabId, SheetManager.MiscTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "TMB Path", Loaded.Path, parentId, SelectResultType.GameAction, Loaded.Name, true );
        }

        protected override string GetName( XivCommon item ) => item.Name;
    }
}