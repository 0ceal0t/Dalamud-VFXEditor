using Dalamud.Bindings.ImGui;
using System.IO;
using VfxEditor.Formats.TextureFormat.Textures;
using VfxEditor.Formats.UldFormat.Texture;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.UldFormat.PartList {
    public class UldPartItem {
        private readonly UldFile File;
        public readonly ParsedShort2 Offset = new( "Offset" );
        public readonly ParsedShort2 Size = new( "Size" );

        public readonly ParsedIntSelect<UldTexture> TextureId;
        public UldTexture CurrentTexture => TextureId.Selected;

        public bool ShowHd = false;

        public UldPartItem( UldFile file ) {
            File = file;
            TextureId = new( "Texture", 0,
                () => File.TextureSplitView,
                item => ( int )item.Id.Value,
                ( item, _ ) => item.GetText()
            );
        }

        public UldPartItem( UldFile file, BinaryReader reader ) : this( file ) {
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

        public TextureDrawable GetTexture( out uint mult ) {
            mult = ShowHd ? 2u : 1u;
            var currentTexture = CurrentTexture;
            if( currentTexture == null ) return null;

            var path = currentTexture.IconId.Value > 0 ? currentTexture.GetIconPath( ShowHd ) : currentTexture.GetTexturePath( ShowHd );
            if( string.IsNullOrEmpty( path ) ) return null;

            return Plugin.TextureManager.GetTexture( path );
        }

        public void DrawImage( bool controls ) {
            var texture = GetTexture( out var mult );
            texture?.Draw(
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
