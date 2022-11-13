using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxSimpleColors : AvfxBase {
        public byte[] Colors = new byte[16];

        public AvfxSimpleColors() : base( "Cols" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < 16; i++ ) Colors[i] = reader.ReadByte();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            for( var i = 0; i < 16; i++ ) writer.Write( Colors[i] );
        }
    }

    public class AvfxSimpleFrames : AvfxBase {
        public int[] Frames = new int[4];

        public AvfxSimpleFrames() : base( "Frms" ) { }

        public override void ReadContents( BinaryReader reader, int size ) {
            for( var i = 0; i < 4; i++ ) Frames[i] = reader.ReadInt16();
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            for( var i = 0; i < 4; i++ ) writer.Write( ( short )Frames[i] );
        }
    }
}
