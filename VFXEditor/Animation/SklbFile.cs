using System.IO;

namespace VfxEditor.Animation {
    public class SklbFile : Lumina.Data.FileResource {
        private byte[] HavokData;

        public override void LoadFile() {
            var size = Reader.BaseStream.Length;

            Reader.BaseStream.Seek( 0, SeekOrigin.Begin );
            Reader.ReadInt32(); // Magic
            var version = Reader.ReadInt32();

            var offsetToHavok = -1;

            if( version == 0x31333030 ) {
                Reader.ReadInt32(); // Skeleton offset
                offsetToHavok = Reader.ReadInt32();
                Reader.ReadInt16();
                Reader.ReadInt16();
                Reader.ReadInt32();
                Reader.ReadInt32();
            }
            else if( version == 0x31323030 ) {
                Reader.ReadInt16(); // Skeleton offset
                offsetToHavok = Reader.ReadInt16();
                Reader.ReadInt16();
                Reader.ReadInt16();
            }

            Reader.BaseStream.Seek( offsetToHavok, SeekOrigin.Begin );
            HavokData = Reader.ReadBytes( ( int )( size - offsetToHavok ) );
        }

        public void SaveHavokData( string path ) => File.WriteAllBytes( path, HavokData );
    }
}
