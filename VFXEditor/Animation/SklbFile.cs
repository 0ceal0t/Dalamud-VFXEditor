using System.IO;

namespace VfxEditor.Animation {
    public class SklbFile : Lumina.Data.FileResource {
        private byte[] HavokData;

        public override void LoadFile() => LoadFile( Reader );

        public void LoadFile( BinaryReader reader ) {
            var size = reader.BaseStream.Length;

            reader.BaseStream.Seek( 0, SeekOrigin.Begin );
            reader.ReadInt32(); // Magic
            var version = reader.ReadInt32();

            var offsetToHavok = -1;

            /* 0x31333031 is found in chara/human/c1801/animation/f0003/resident/face.pap
             * It has a different structure than 0x31333030, but this works for now
             */

            if( version == 0x31333030 || version == 0x31333031 ) {
                reader.ReadInt32(); // Skeleton offset
                offsetToHavok = reader.ReadInt32();
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt32();
                reader.ReadInt32();
            }
            else if( version == 0x31323030 ) {
                reader.ReadInt16(); // Skeleton offset
                offsetToHavok = reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadInt16();
            }

            reader.BaseStream.Seek( offsetToHavok, SeekOrigin.Begin );
            HavokData = reader.ReadBytes( ( int )( size - offsetToHavok ) );
        }

        public void SaveHavokData( string path ) => File.WriteAllBytes( path, HavokData );

        public static SklbFile LoadFromLocal( string path ) {
            var sklb = new SklbFile();
            var file = File.Open( path, FileMode.Open );
            using( var reader = new BinaryReader( file ) ) {
                sklb.LoadFile( reader );
            }
            file.Close();
            return sklb;
        }
    }
}
