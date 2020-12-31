using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVFXLib.Models
{
    public class EmitVertex
    {
        public static int SIZE = 28;

        public float[] Position = new float[3]; // 4 bytes = 12
        public float[] Normal = new float[3]; // 4 bytes = 12, starts at 12
        public int C; // 4 bytes, starts at 24
        // total = 28

        public EmitVertex() { }

        public EmitVertex(byte[] bytes)
        {
            for (int idx = 0; idx < 3; idx++)
            {
                byte[] pBytes = new byte[4];
                byte[] nBytes = new byte[4];
                Buffer.BlockCopy(bytes, 0 + 4 * idx, pBytes, 0, 4);
                Buffer.BlockCopy(bytes, 12 + 4 * idx, nBytes, 0, 4);
                Position[idx] = Util.Bytes4ToFloat(pBytes);
                Normal[idx] = Util.Bytes4ToFloat(nBytes);
            }
            byte[] cBytes = new byte[8];
            Buffer.BlockCopy(bytes, 24, cBytes, 0, 4);
            C = Util.Bytes4ToInt(cBytes);
        }

        public byte[] toBytes()
        {
            byte[] bytes = new byte[SIZE];

            for (int idx = 0; idx < 3; idx++)
            {
                byte[] pBytes = Util.FloatTo4Bytes(Position[idx]);
                byte[] nBytes = Util.FloatTo4Bytes(Normal[idx]);
                Buffer.BlockCopy(pBytes, 0, bytes, 0 + 4 * idx, 4);
                Buffer.BlockCopy(nBytes, 0, bytes, 12 + 4 * idx, 4);
            }
            byte[] cBytes = Util.IntTo4Bytes(C);
            Buffer.BlockCopy(cBytes, 0, bytes, 24, 4);

            return bytes;
        }
    }
}
