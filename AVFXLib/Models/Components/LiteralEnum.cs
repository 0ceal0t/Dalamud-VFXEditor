using AVFXLib.AVFX;
using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class LiteralEnum<T> : LiteralBase
    {
        public T Value { get; set; }
        public string[] Options = Enum.GetNames(typeof(T));

        public LiteralEnum(string jsonPath, string avfxName, int size = 4) : base(jsonPath, avfxName, size)
        {
        }

        public override void read(AVFXNode node)
        {
        }

        public override void read(AVFXLeaf leaf)
        {
            int intValue = Util.Bytes4ToInt(leaf.Contents);
            if (intValue != -1) // means none
            {
                Value = (T)(object)intValue;
            }

            Size = leaf.Size;
            Assigned = true;
        }

        public void GiveValue(string value)
        {
            T enumValue = (T)Enum.Parse(typeof(T), value, true);
            GiveValue(enumValue);
        }
        public void GiveValue(T value)
        {
            Value = value;
            Assigned = true;
        }

        public override void toDefault()
        {
            GiveValue((T)(object)0);
        }

        public override JToken toJSON()
        {
            return new JValue(stringValue());
        }

        public override AVFXNode toAVFX()
        {
            int enumValue = -1;
            if(Value != null)
            {
                enumValue = (int)(object)Value;
            }
            return new AVFXLeaf(AVFXName, Size, Util.IntTo4Bytes(enumValue));
        }

        public override string stringValue()
        {
            return Value.ToString();
        }
    }
}
