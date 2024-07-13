using System.IO;
using VfxEditor.Formats.PhybFormat.Extended.Ephb;
using VFXEditor.Flatbuffer.Ephb;

namespace VfxEditor.Formats.PhybFormat.Extended {
    public class PhybExtended {
        public const uint MAGIC_PACK = 0x4B434150;
        public const uint MAGIC_EPHB = 0x42485045;

        private readonly bool Padded = true;

        public readonly PhybEphbTable Table;

        /*
         *       ..\flatc.exe --csharp --gen-object-api --gen-onefile .\Ephb.fbs
         */

        public PhybExtended() { }

        public PhybExtended( BinaryReader reader, out long startPos, out int size ) : this() {
            // already read PACK
            reader.BaseStream.Position += 0xC; // skip to [offset]
            var offset = reader.ReadInt64();
            reader.BaseStream.Position -= offset;
            if( reader.ReadUInt32() != 0 ) { // sometimes there's a zero
                reader.BaseStream.Position -= 4; // TODO: double-check this
                Padded = false;
            }
            startPos = reader.BaseStream.Position;

            reader.ReadUInt32(); // Magic
            reader.ReadUInt32(); // 1
            size = reader.ReadInt32();
            reader.ReadUInt32(); // 0
            Table = new( EphbTable.GetRootAsEphbTable( new( reader.ReadBytes( size ) ) ).UnPack() );

            // back to PACK
        }

        public void Write( BinaryWriter writer ) {
            var data = Table.Export().SerializeToBinary();

            if( Padded ) writer.Write( 0 );

            var startPos = writer.BaseStream.Position;
            writer.Write( MAGIC_EPHB );
            writer.Write( ( ushort )1 ); // version
            writer.Write( ( ushort )0 ); // count
            writer.Write( ( long )data.Length ); // offset_next
            writer.Write( data );

            writer.Write( MAGIC_PACK );
            writer.Write( ( ushort )1 ); // version
            writer.Write( ( ushort )1 ); // count
            writer.Write( startPos - writer.BaseStream.Position + 0x8 ); // offset to first
            writer.Write( writer.BaseStream.Position - startPos + 0x8 ); // total size
        }

        public void Draw() => Table.Draw();
    }
}
