using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedReserve : ParsedBase {
        private readonly int Size = 0;
        private byte[] Data;

        public ParsedReserve( int size ) {
            Size = size;
            Data = new byte[Size];
        }

        public override void Draw( CommandManager manager ) { }

        public override void Read( BinaryReader reader ) {
            Data = reader.ReadBytes( Size );
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Write( BinaryWriter writer ) => writer.Write( Data );
    }
}
