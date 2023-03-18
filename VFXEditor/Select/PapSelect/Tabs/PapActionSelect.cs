using ImGuiScene;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.PapSelect {
    public class PapActionSelect : SelectTab<XivActionPap, XivActionPapSelected> {
        private TextureWrap Icon;

        public PapActionSelect( string tabId, PapSelectDialog dialog, bool nonPlayer = false ) : base( tabId, nonPlayer ? SheetManager.NonPlayerActionPap : SheetManager.ActionPap, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );

            DrawPapDict( Loaded.StartAnimations, "Start", Loaded.ActionPap.Name, $"{parentId}/Start" );
            DrawPapDict( Loaded.EndAnimations, "End", Loaded.ActionPap.Name, $"{parentId}/End" );
            DrawPapDict( Loaded.HitAnimations, "Hit", Loaded.ActionPap.Name, $"{parentId}/Hit" );
        }

        protected override string GetName( XivActionPap item ) => item.Name;
    }
}
