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
    public class AVFXLife : Base
    {
        public const string NAME = "Life";

        public bool Enabled;

        public LiteralFloat Value = new LiteralFloat("value", "Val");
        public LiteralFloat ValRandom = new LiteralFloat("random", "ValR");
        public LiteralEnum<RandomType> ValRandomType = new LiteralEnum<RandomType>("randomType", "Type");

        List<Base> Attributes;

        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public AVFXLife(string jsonPath) : base(jsonPath, NAME)
        {
            Attributes = new List<Base>(new Base[]{
                Value,
                ValRandom,
                ValRandomType
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            Enabled = (node.Children.Count > 2);
            if (Enabled)
            {
                ReadAVFX(Attributes, node);
            }
        }

        public override void toDefault()
        {
            Assigned = true;
            Enabled = true;
            SetDefault(Attributes);
            Value.GiveValue(-1f);
        }

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            if (Enabled)
            {
                PutJSON(elem, Attributes);
            }
            elem["enabled"] = new JValue(Enabled);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            if (Enabled)
            {
                AVFXNode lifeAvfx = new AVFXNode("Life");
                PutAVFX(lifeAvfx, Attributes);

                return lifeAvfx;

            }
            else // -1
            {
                AVFXNode lifeAvfx = new AVFXLeaf("Life", 4, Util.IntTo4Bytes(-1));
                return lifeAvfx;
            }
        }
    }
}
