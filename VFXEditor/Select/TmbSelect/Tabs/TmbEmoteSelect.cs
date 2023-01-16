using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.TmbSelect {
    public class TmbEmoteSelect : SelectTab<XivEmoteTmb> {
        private ImGuiScene.TextureWrap Icon;

        public TmbEmoteSelect( string tabId, TmbSelectDialog dialog ) : base( tabId, SheetManager.EmoteTmb, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "Path", Selected.TmbFiles, parentId, SelectResultType.GameEmote, Selected.Name, true );
        }

        protected override string GetName( XivEmoteTmb item ) => item.Name;
    }
}
