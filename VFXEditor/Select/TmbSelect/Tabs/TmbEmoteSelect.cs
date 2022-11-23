using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbEmoteSelect : SelectTab<XivEmoteTmb, XivEmoteTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbEmoteSelect( string tabId, TmbSelectDialog dialog ) : base( tabId, SheetManager.EmoteTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "Tmb Path", Loaded.TmbFiles, parentId, SelectResultType.GameEmote, Loaded.Name, true );
        }

        protected override string GetName( XivEmoteTmb item ) => item.Name;
    }
}
