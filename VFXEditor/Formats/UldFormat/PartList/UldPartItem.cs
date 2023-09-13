using ImGuiNET;
using System.IO;
using System.Linq;
using VfxEditor.Parsing;
using VfxEditor.UldFormat.Texture;

namespace VfxEditor.UldFormat.PartList {
    public class UldPartItem {
        public readonly ParsedUInt TextureId = new( "Texture Id" );
        private readonly ParsedUInt U = new( "U", size: 2 );
        private readonly ParsedUInt V = new( "V", size: 2 );
        private readonly ParsedUInt W = new( "W", size: 2 );
        private readonly ParsedUInt H = new( "H", size: 2 );

        public UldTexture CurrentTexture => Plugin.UldManager.CurrentFile.Textures.Where( x => x.Id.Value == TextureId.Value ).FirstOrDefault();

        private bool ShowHd = false;

        public UldPartItem() { }

        public UldPartItem( BinaryReader reader ) {
            TextureId.Read( reader );
            U.Read( reader );
            V.Read( reader );
            W.Read( reader );
            H.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TextureId.Write( writer );
            U.Write( writer );
            V.Write( writer );
            W.Write( writer );
            H.Write( writer );
        }

        public void Draw() {
            TextureId.Draw( CommandManager.Uld );

            ImGui.Checkbox( "Show HD", ref ShowHd );

            var currentTexture = CurrentTexture;
            if( currentTexture != null ) {
                var path = currentTexture.IconId.Value > 0 ? currentTexture.GetIconPath( ShowHd ) : currentTexture.GetTexturePath( ShowHd );
                var mult = ShowHd ? 2u : 1u;

                if( !string.IsNullOrEmpty( path ) ) {
                    Plugin.TextureManager.GetTexture( path )?.Draw(
                        U.Value * mult,
                        V.Value * mult,
                        W.Value * mult,
                        H.Value * mult
                    );
                }
            }

            U.Draw( CommandManager.Uld );
            V.Draw( CommandManager.Uld );
            W.Draw( CommandManager.Uld );
            H.Draw( CommandManager.Uld );
        }
    }
}
