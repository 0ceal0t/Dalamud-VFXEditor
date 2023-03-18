using ImGuiNET;
using ImGuiScene;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.ScdSelect {
    public class ScdContentSelect : SelectTab<XivInstanceContent, XivInstanceContentSelected> {
        private TextureWrap Icon;

        public ScdContentSelect( string tabId, ScdSelectDialog dialog ) : base( tabId, SheetManager.Content, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Image, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawBgmSituation( Loaded.Content.Name, parentId, Loaded.Situation );
        }

        protected override string GetName( XivInstanceContent item ) => item.Name;
    }
}