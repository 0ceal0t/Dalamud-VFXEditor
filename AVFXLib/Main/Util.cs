using AVFXLib.AVFX;
using AVFXLib.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Main
{
    public class Util
    {
        // BYTE STUFF
        public static void PrintBytes(byte[] b)
        {
            string s = BitConverter.ToString(b);
            Console.WriteLine(BitConverter.ToString(b).Replace("-", ""));
        }

        public static byte[] rb(byte[] b)
        {
            return b.Reverse().ToArray();
        }

        public static byte[] IntTo2Bytes(int intVal)
        {
            return BitConverter.GetBytes(Convert.ToInt16(intVal));
        }
        public static int Bytes2ToInt(byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte[] IntTo1Bytes(int intVal)
        {
            return new byte[] { Convert.ToByte(intVal) };
        }
        public static int Bytes1ToInt(byte[] bytes)
        {
            return (int)bytes[0];
        }

        public static byte[] IntTo4Bytes(int intVal)
        {
            return BitConverter.GetBytes(Convert.ToInt32(intVal));
        }
        public static int Bytes4ToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] FloatTo4Bytes(float floatVal)
        {
            return BitConverter.GetBytes(floatVal);
        }
        public static float Bytes4ToFloat(byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }

        public static byte[] FloatTo2Bytes(float floatVal)
        {
            ushort ushortVal = Pack(floatVal);
            return BitConverter.GetBytes(ushortVal);
        }
        public static float Bytes2ToFloat(byte[] bytes)
        {
            return Unpack(bytes, 0);
        }

        public static byte[] BoolTo4Bytes(bool? boolVal)
        {
            if(boolVal == null)
            {
                return new byte[] {0xff, 0x00, 0x00, 0x00};
            }
            else if(boolVal == true)
            {
                return new byte[] { 0x01, 0x00, 0x00, 0x00 };
            }
            else
            {
                return new byte[] { 0x00, 0x00, 0x00, 0x00 };
            }
        }
        public static bool? Bytes4ToBool(byte[] bytes)
        {
            if(bytes[0] == 0xff)
            {
                return null;
            }
            else if(bytes[0] == 0x01)
            {
                return true;
            }
            else if(bytes[0] == 0x00)
            {
                return false;
            }
            return null;
        }

        public static byte[] BoolTo1Bytes(bool? boolVal)
        {
            if (boolVal == null)
            {
                return new byte[] { 0xff};
            }
            else if (boolVal == true)
            {
                return new byte[] { 0x01 };
            }
            else
            {
                return new byte[] { 0x00};
            }
        }
        public static bool? Bytes1ToBool(byte[] bytes)
        {
            if (bytes[0] == 0xff)
            {
                return null;
            }
            else if (bytes[0] == 0x01)
            {
                return true;
            }
            else if (bytes[0] == 0x00)
            {
                return false;
            }
            return null;
        }

        public static byte[] StringToBytes(string stringVal)
        {
            return Encoding.ASCII.GetBytes(stringVal);
        }
        public static string BytesToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        // MISC
        public static int RoundUp(int v)
        {
            return 4 * (int)Math.Ceiling(v / 4.0);
        }

        public static byte[] NameTo4Bytes(string name)
        {
            byte[] nameBytes = StringToBytes(name);
            byte[] ret = new byte[] { 0, 0, 0, 0 };
            for(int i = 0; i < nameBytes.Length; i++)
            {
                ret[i] = nameBytes[nameBytes.Length - i - 1];
            }
            return ret;
        }

        public static byte[][] SplitBytes(byte[] bytes, int size)
        {
            int numElems = (int)Math.Floor((double)bytes.Length / size);
            byte[][] ret = new byte[numElems][];
            for(int idx = 0; idx < numElems; idx++)
            {
                byte[] item = new byte[size];
                Buffer.BlockCopy(bytes, idx * size, item, 0, size);
                ret[idx] = item;
            }
            return ret;
        }
        public static byte[] JoinBytes(byte[][] elems, int size)
        {
            byte[] ret = new byte[elems.Length * size];
            int idx = 0;
            foreach(byte[] bytes in elems)
            {
                Buffer.BlockCopy(bytes, 0, ret, idx * size, size);
                idx++;
            }
            return ret;
        }

        // HELPERS
        public static unsafe ushort Pack(float value)
        {
            var num5 = *((uint*)&value);
            var num3 = (uint)((num5 & -2147483648) >> 0x10);
            var num = num5 & 0x7fffffff;
            if (num > 0x47ffefff)
            {
                return (ushort)(num3 | 0x7fff);
            }
            if (num >= 0x38800000) return (ushort)(num3 | ((((num + -939524096) + 0xfff) + ((num >> 13) & 1)) >> 13));

            var num6 = (num & 0x7fffff) | 0x800000;
            var num4 = 0x71 - ((int)(num >> 0x17));
            num = (num4 > 0x1f) ? 0 : (num6 >> num4);
            return (ushort)(num3 | (((num + 0xfff) + ((num >> 13) & 1)) >> 13));
        }

        public static float Unpack(byte[] buffer, int offset)
        {
            return Unpack(BitConverter.ToUInt16(buffer, offset));
        }

        public static unsafe float Unpack(ushort value)
        {
            uint num3;
            if ((value & -33792) == 0)
            {
                if ((value & 0x3ff) != 0)
                {
                    var num2 = 0xfffffff2;
                    var num = (uint)(value & 0x3ff);
                    while ((num & 0x400) == 0)
                    {
                        num2--;
                        num = num << 1;
                    }
                    num &= 0xfffffbff;
                    num3 = ((uint)(((value & 0x8000) << 0x10) | ((num2 + 0x7f) << 0x17))) | (num << 13);
                }
                else
                {
                    num3 = (uint)((value & 0x8000) << 0x10);
                }
            }
            else
            {
                num3 =
                    (uint)
                    ((((value & 0x8000) << 0x10) | (((((value >> 10) & 0x1f) - 15) + 0x7f) << 0x17))
                     | ((value & 0x3ff) << 13));
            }
            return *(((float*)&num3));
        }
    }
}
