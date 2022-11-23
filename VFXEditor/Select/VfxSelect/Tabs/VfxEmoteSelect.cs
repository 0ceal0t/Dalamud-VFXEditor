using ImGuiNET;
using VfxEditor.Select.Rows;

namespace VfxEditor.Select.VfxSelect {
    public class VfxEmoteSelect : SelectTab<XivEmote, XivEmoteSelected> {
        private ImGuiScene.TextureWrap Icon;

        public VfxEmoteSelect( string tabId, VfxSelectDialog dialog ) : base( tabId, SheetManager.Emotes, dialog ) { }

        protected override void OnSelect() => LoadIcon( Selected.Icon, ref Icon );

        protected override void DrawSelected( string parentId ) {
            DrawIcon( Icon );
            DrawPath( "VFX", Loaded.VfxPaths, parentId, SelectResultType.GameEmote, Loaded.Emote.Name, true );
        }

        protected override string GetName( XivEmote item ) => item.Name;
    }
}