using ImGuiNET;
using System.IO;
using VfxEditor.Parsing.Int;
using VfxEditor.UldFormat.Texture;

namespace VfxEditor.UldFormat.PartList {
    public class UldPartItem {
        private readonly ParsedShort2 Offset = new( "Offset" );
        private readonly ParsedShort2 Size = new( "Size" );

        public readonly ParsedIntSelect<UldTexture> TextureId = new( "Texture", 0,
            () => Plugin.UldManager.File.TextureSplitView,
            ( UldTexture item ) => ( int )item.Id.Value,
            ( UldTexture item, int _ ) => item.GetText()
        );
        public UldTexture CurrentTexture => TextureId.Selected;

        private bool ShowHd = false;

        public UldPartItem() { }

        public UldPartItem( BinaryReader reader ) {
            TextureId.Read( reader );
            Offset.Read( reader );
            Size.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TextureId.Write( writer );
            Offset.Write( writer );
            Size.Write( writer );
        }

        public void Draw() {
            ImGui.Checkbox( "Show HD", ref ShowHd );

            DrawImage( true );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            TextureId.Draw();
            Offset.Draw();
            Size.Draw();
        }

        public void DrawImage( bool controls ) {
            var currentTexture = CurrentTexture;
            if( currentTexture == null ) return;

            var path = currentTexture.IconId.Value > 0 ? currentTexture.GetIconPath( ShowHd ) : currentTexture.GetTexturePath( ShowHd );
            if( string.IsNullOrEmpty( path ) ) return;

            var mult = ShowHd ? 2u : 1u;
            Plugin.TextureManager.GetTexture( path )?.Draw(
                ( uint )Offset.Value.X * mult,
                ( uint )Offset.Value.Y * mult,
                ( uint )Size.Value.X * mult,
                ( uint )Size.Value.Y * mult,
                controls
            );
        }

        public string GetText( int idx ) {
            var currentTexture = CurrentTexture;
            var text = currentTexture != null ? currentTexture.GetText() : $"Texture {TextureId.Value}";
            return $"Part {idx} ({text})";
        }
    }
}
