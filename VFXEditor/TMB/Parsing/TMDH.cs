using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Parsing {
    public class TMDH {
        public static int Size => 0x10;

        private short Unk1;
        private short Unk2;
        private short Unk3;

        public TMDH( BinaryReader reader ) {
            reader.ReadInt32(); // TMDH
            reader.ReadInt32(); // 0x10
            reader.ReadInt16(); // id
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt16(); // ?
            Unk3 = reader.ReadInt16(); // 3
        }

        public void Write( BinaryWriter writer ) {
            FileHelper.WriteString( writer, "TMDH" );
            writer.Write( Size );
            writer.Write( ( short )1 );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Unknown 1{id}", ref Unk1 );
            FileHelper.ShortInput( $"Unknown 2{id}", ref Unk2 );
            FileHelper.ShortInput( $"Unknown 3{id}", ref Unk3 );
        }
    }
}
