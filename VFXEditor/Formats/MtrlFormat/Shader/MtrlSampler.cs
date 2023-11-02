using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public class MtrlSampler : IUiItem {
        public readonly ParsedCrc Id = new( "Id" );
        public readonly ParsedUInt Flags = new( "Flags" );
        public readonly ParsedByte TextureIndex = new( "Texture Index" );
        private readonly ParsedReserve Padding = new( 3 );

        public MtrlSampler() { }

        public MtrlSampler( BinaryReader reader ) {
            Id.Read( reader );
            Flags.Read( reader );
            TextureIndex.Read( reader );
            Padding.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            Flags.Write( writer );
            TextureIndex.Write( writer );
            Padding.Write( writer );
        }

        public void Draw() {
            Id.Draw( CrcMaps.MaterialParams ); // TODO
            Flags.Draw();
            TextureIndex.Draw();
        }
    }
}
