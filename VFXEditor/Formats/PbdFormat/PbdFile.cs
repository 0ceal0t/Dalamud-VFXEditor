using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdFile : FileManagerFile {
        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Files/PbdFile.cs
        // https://github.com/Ottermandias/Penumbra.GameData/blob/f5a74c70ad3861c5c66e1df6ae9a29fc7a0d736a/Data/RacialDeformer.cs#L7

        public readonly List<PbdDeformer> Deformers = [];
        public readonly List<PdbConnection> Connections = [];

        public PbdFile( BinaryReader reader, bool verify ) : base() {
            var count = reader.ReadInt32();
            for( var i = 0; i < count; i++ ) Deformers.Add( new( reader ) );
            for( var i = 0; i < count; i++ ) Connections.Add( new( Deformers, reader ) );
        }

        public override void Write( BinaryWriter writer ) {

        }

        public override void Draw() {

        }
    }
}
