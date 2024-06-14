using System.IO;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public const uint MAGIC_PACK = 0x4B434150;
        public const uint MAGIC_EPHB = 0x42485045;

        public readonly byte[] PackData;
        public readonly PhybEphbTable Table;

        /*
         * ..\flatc.exe --csharp --gen-object-api --gen-onefile .\Ephb.fbs
         */

        public PhybExtended() { }

        public PhybExtended( BinaryReader reader ) : this() {
            reader.ReadUInt32(); // 1
            var size = reader.ReadUInt32();
            reader.ReadUInt32(); // 0
            Table = new( EphbTable.GetRootAsEphbTable( new( reader.ReadBytes( ( int )size ) ) ).UnPack() );

            PackData = reader.ReadBytes( 0x18 );
            // TODO: pack data
        }

        public void Write( BinaryWriter writer ) {
            var data = Table.Export().SerializeToBinary();

            writer.Write( MAGIC_EPHB );
            writer.Write( 1 );
            writer.Write( data.Length );
            writer.Write( 0 );
            writer.Write( data );
            writer.Write( PackData );
        }

        public void Draw() => Table.Draw();
    }
}
