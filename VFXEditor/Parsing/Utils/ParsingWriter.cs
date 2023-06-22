using System.IO;

namespace VfxEditor.Parsing.Utils {
    public class ParsingWriter {
        public readonly BinaryWriter Writer;
        public readonly MemoryStream WriterMs;

        public long Position => Writer.BaseStream.Position;

        public ParsingWriter() {
            WriterMs = new();
            Writer = new( WriterMs );
        }

        public virtual void Dispose() {
            Writer.Close();
            WriterMs.Close();
        }

        public void Seek( long pos ) => Writer.BaseStream.Seek( pos, SeekOrigin.Begin );

        public void Write( uint data ) => Writer.Write( data );
        public void Write( int data ) => Writer.Write( data );
        public void Write( short data ) => Writer.Write( data );
        public void Write( ushort data ) => Writer.Write( data );
        public void Write( byte data ) => Writer.Write( data );
        public void Write( float data ) => Writer.Write( data );

        public virtual void WriteTo( BinaryWriter writer ) {
            writer.Write( WriterMs.ToArray() );
        }
    }
}
