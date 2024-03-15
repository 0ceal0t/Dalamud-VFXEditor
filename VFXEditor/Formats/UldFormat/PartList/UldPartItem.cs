using ImGuiNET;
using System.IO;
using System.Linq;
using VfxEditor.Parsing.Int;
using VfxEditor.UldFormat.Texture;

namespace VfxEditor.UldFormat.PartList {
    public class UldPartItem {
        private readonly ParsedShort2 Offset = new( "Offset" );
        private readonly ParsedShort2 Size = new( "Size" );

        public readonly ParsedItemSelect<UldTexture> TextureId = new( "Texture",
            () => Plugin.UldManager.File.TextureSplitView, ( UldTexture item ) => ( int )item.Id.Value, 0 );

        public UldTexture CurrentTexture => Plugin.UldManager.File.Textures.FirstOrDefault( x => x.Id.Value == TextureId.Value, null );

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

            var currentTexture = CurrentTexture;
            if( currentTexture != null ) {
                var path = currentTexture.IconId.Value > 0 ? currentTexture.GetIconPath( ShowHd ) : currentTexture.GetTexturePath( ShowHd );
                var mult = ShowHd ? 2u : 1u;

                if( !string.IsNullOrEmpty( path ) ) {
                    Plugin.TextureManager.GetTexture( path )?.Draw(
                        ( uint )Offset.Value.X * mult,
                        ( uint )Offset.Value.Y * mult,
                        ( uint )Size.Value.X * mult,
                        ( uint )Size.Value.Y * mult
                    );
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            TextureId.Draw();
            Offset.Draw();
            Size.Draw();
        }
    }
}
