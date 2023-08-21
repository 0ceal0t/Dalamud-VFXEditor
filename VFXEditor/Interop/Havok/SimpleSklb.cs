using System.IO;

namespace VfxEditor.Interop.Havok {
    public class SimpleSklb : Lumina.Data.FileResource {
        private byte[] HavokData;

        public override void LoadFile() => LoadFile( Reader );

        public void LoadFile( BinaryReader reader ) {
            var size = reader.BaseStream.Length;

            reader.BaseStream.Seek( 0, SeekOrigin.Begin );
            reader.ReadInt32(); // Magic
            var version = reader.ReadInt32();

            var havokOffset = -1;

            /* 0x31333031 is found in chara/human/c1801/animation/f0003/resident/face.pap
             * It has a different structure than 0x31333030, but this works for now
             */

            if( version == 0x31333030 || version == 0x31333031 ) {
                reader.ReadInt32();
                havokOffset = reader.ReadInt32();
            }
            else if( version == 0x31323030 ) {
                reader.ReadInt16();
                havokOffset = reader.ReadInt16();
            }

            reader.BaseStream.Seek( havokOffset, SeekOrigin.Begin );
            HavokData = reader.ReadBytes( ( int )( size - havokOffset ) );
        }

        public void SaveHavokData( string path ) => File.WriteAllBytes( path, HavokData );

        public static SimpleSklb LoadFromLocal( string path ) {
            var sklb = new SimpleSklb();
            var file = File.Open( path, FileMode.Open );
            using( var reader = new BinaryReader( file ) ) {
                sklb.LoadFile( reader );
            }
            file.Close();
            return sklb;
        }
    }
}
