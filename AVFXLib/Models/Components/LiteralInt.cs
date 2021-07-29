using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class LiteralInt : LiteralBase
    {
        public int Value { get; set; }

        public LiteralInt(string avfxName, int size = 4) : base(avfxName, size)
        {
        }

        public override void Read(AVFXNode node)
        {
        }

        public override void read( AVFXLeaf leaf)
        {
            Size = leaf.Size;
            if (Size == 4)
            {
                Value = BitConverter.ToInt32(leaf.Contents, 0);
            }
            else if (Size == 1)
            {
                Value = (int)leaf.Contents[0];
            }
            Assigned = true;
        }

        public void GiveValue(int value)
        {
            Value = value;
            Assigned = true;
        }

        public override void ToDefault()
        {
            GiveValue(0);
        }

        public override AVFXNode ToAVFX()
        {
            var bytes = new byte[0];
            if (Size == 4)
            {
                bytes = BitConverter.GetBytes(Convert.ToInt32(Value));
            }
            else if (Size == 1)
            {
                bytes = new byte[] { Convert.ToByte(Value) };
            }
            return new AVFXLeaf(AVFXName, Size, bytes);
        }

        public override string stringValue()
        {
            return Value.ToString();
        }
    }
}
