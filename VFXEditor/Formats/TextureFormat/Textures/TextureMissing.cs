using Dalamud.Bindings.ImGui;
using VfxEditor.Utils;

namespace VfxEditor.Formats.TextureFormat.Textures {
    public class TextureMissing : TextureDrawable {
        public TextureMissing( string gamePath ) : base( gamePath ) { }

        public override void DrawFullImage() { }

        public override void DrawImage() { }

        public override void DrawImage( uint u, uint v, uint w, uint h ) { }

        public override void DrawImage( float height ) { }

        protected override void DrawControls() {
            ImGui.TextColored( UiUtils.RED_COLOR, "Texture is missing" );
            if( ImGui.Button( "Import" ) ) ImportDialog();
            DrawSettingsCog();
            DrawSettingsPopup();
        }

        protected override TexturePreview GetPreview() => null;

        protected override TextureDataFile GetRawData() => null;

        protected override void OnReplace( string importPath ) => Plugin.TextureManager.ReplaceTexture( importPath, GamePath );
    }
}
