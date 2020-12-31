using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVFXLib.Models
{
    public class Index
    {
        public static int SIZE = 6;

        public int I1; // 2 bytes
        public int I2; // 2 bytes
        public int I3; // 2 bytes

        public Index(byte[] bytes)
        {
            byte[] i1 = new byte[2];
            byte[] i2 = new byte[2];
            byte[] i3 = new byte[2];
            Buffer.BlockCopy(bytes, 0, i1, 0, 2);
            Buffer.BlockCopy(bytes, 2, i2, 0, 2);
            Buffer.BlockCopy(bytes, 4, i3, 0, 2);
            I1 = Util.Bytes2ToInt(i1);
            I2 = Util.Bytes2ToInt(i2);
            I3 = Util.Bytes2ToInt(i3);
        }

        public byte[] toBytes()
        {
            byte[] bytes = new byte[SIZE];
            byte[] i1 = Util.IntTo2Bytes(I1);
            byte[] i2 = Util.IntTo2Bytes(I2);
            byte[] i3 = Util.IntTo2Bytes(I3);
            Buffer.BlockCopy(i1, 0, bytes, 0, 2);
            Buffer.BlockCopy(i2, 0, bytes, 2, 2);
            Buffer.BlockCopy(i3, 0, bytes, 4, 2);
            return bytes;
        }
    }
}
