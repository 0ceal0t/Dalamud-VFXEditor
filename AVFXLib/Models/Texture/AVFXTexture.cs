using AVFXLib.AVFX;
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

        public LiteralString Path = new(NAME);

        public AVFXTexture() : base(NAME)
        {
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;

            var leaf = (AVFXLeaf)node;
            var Value = Encoding.ASCII.GetString(leaf.Contents);
            Path.GiveValue(Value);
        }

        public override void ToDefault()
        {
            Path.GiveValue("");
        }

        public override AVFXNode ToAVFX()
        {
            return Path.ToAVFX();
        }
    }
}
