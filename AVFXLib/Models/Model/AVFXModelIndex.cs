using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVFXLib.Models {
    public class Index {
        public static readonly int SIZE = 6;

        public int I1; // 2 bytes
        public int I2; // 2 bytes
        public int I3; // 2 bytes

        public Index() { }

        public Index( byte[] bytes ) {
            var i1 = new byte[2];
            var i2 = new byte[2];
            var i3 = new byte[2];
            Buffer.BlockCopy( bytes, 0, i1, 0, 2 );
            Buffer.BlockCopy( bytes, 2, i2, 0, 2 );
            Buffer.BlockCopy( bytes, 4, i3, 0, 2 );
            I1 = Util.Bytes2ToInt( i1 );
            I2 = Util.Bytes2ToInt( i2 );
            I3 = Util.Bytes2ToInt( i3 );
        }

        public byte[] ToBytes() {
            var bytes = new byte[SIZE];
            var i1 = Util.IntTo2Bytes( I1 );
            var i2 = Util.IntTo2Bytes( I2 );
            var i3 = Util.IntTo2Bytes( I3 );
            Buffer.BlockCopy( i1, 0, bytes, 0, 2 );
            Buffer.BlockCopy( i2, 0, bytes, 2, 2 );
            Buffer.BlockCopy( i3, 0, bytes, 4, 2 );
            return bytes;
        }
    }
}
