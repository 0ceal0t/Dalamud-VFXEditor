using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTexture : Base
    {
        public const string NAME = "Tex";

        public LiteralString Path = new LiteralString("path", NAME);

        public AVFXTexture() : base("path", NAME)
        {
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;

            AVFXLeaf leaf = (AVFXLeaf)node;
            string Value = Encoding.ASCII.GetString(leaf.Contents);
            Path.GiveValue(Value);
        }

        public override void toDefault()
        {
            Path.GiveValue("");
        }

        public override JToken toJSON()
        {
            return Path.toJSON();
        }

        public override AVFXNode toAVFX()
        {
            return Path.toAVFX();
        }
    }
}
