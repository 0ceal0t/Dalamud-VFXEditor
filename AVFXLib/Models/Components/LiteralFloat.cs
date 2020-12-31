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
    public class LiteralFloat : LiteralBase
    {
        public float Value { get; set; }

        public LiteralFloat(string jsonPath, string avfxName, int size = 4) : base(jsonPath, avfxName, size)
        {
        }

        public override void read(AVFXNode node)
        {
        }

        public override void read(AVFXLeaf leaf)
        {
            Value = Util.Bytes4ToFloat(leaf.Contents);
            Size = leaf.Size;
            Assigned = true;
        }

        public void GiveValue(float value)
        {
            Value = value;
            Assigned = true;
        }

        public override void toDefault()
        {
            GiveValue(0.0f);
        }

        public override JToken toJSON()
        {
            return new JValue(Value);
        }

        public override AVFXNode toAVFX()
        {
            return new AVFXLeaf(AVFXName, Size, Util.FloatTo4Bytes(Value));
        }

        public override string stringValue()
        {
            return Value.ToString();
        }
    }
}
