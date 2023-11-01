using System.IO;
using VfxEditor.Formats.ShpkFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MtrlFormat.Shader {
    public class MtrlSampler : IUiItem {
        private readonly ParsedCrc Id = new( "Id" );
        private readonly ParsedUInt Flags = new( "Flags" );
        private readonly ParsedByte TextureIndex = new( "Texture Index" );
        private readonly ParsedReserve Padding = new( 3 );

        public MtrlSampler() { }

        public MtrlSampler( BinaryReader reader ) {
            Id.Read( reader );
            Flags.Read( reader );
            TextureIndex.Read( reader );
            Padding.Read( reader );
        }

        public void Writer( BinaryWriter writer ) {
            // TODO
        }

        public void Draw() {
            Id.Draw( CrcMaps.MaterialParams ); // TODO
            Flags.Draw();
            TextureIndex.Draw();
        }
    }
}
