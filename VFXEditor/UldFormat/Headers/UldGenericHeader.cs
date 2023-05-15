using System.IO;

namespace VfxEditor.UldFormat.Headers {
    public class UldGenericHeader {
        public readonly char[] Identifier;
        public readonly char[] Version;

        public UldGenericHeader( string identifier, string version ) {
            Identifier = identifier.ToCharArray();
            Version = version.ToCharArray();
        }

        public UldGenericHeader( BinaryReader reader ) {
            Identifier = reader.ReadChars( 4 );
            Version = reader.ReadChars( 4 );
        }

        protected void WriteHeader( BinaryWriter writer ) {
            writer.Write( Identifier );
            writer.Write( Version );
        }
    }
}
