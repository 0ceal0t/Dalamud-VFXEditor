using AVFXLib.Main;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVFXLib.Models
{
    public class VNum
    {
        public static int SIZE = 2;

        public int Num; // 2 bytes

        public VNum(int elem)
        {
            Num = elem;
        }

        public VNum(byte[] bytes)
        {
            Num = Util.Bytes2ToInt(bytes);
        }

        public byte[] toBytes()
        {
            return Util.IntTo2Bytes(Num);
        }
    }
}
