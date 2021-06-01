using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXTextureReflection : Base
    {
        public LiteralBool Enabled = new LiteralBool("bEna");
        public LiteralBool UseScreenCopy = new LiteralBool("bUSC");
        public LiteralEnum<TextureFilterType> TextureFilter = new LiteralEnum<TextureFilterType>("TFT");
        public LiteralEnum<TextureCalculateColor> TextureCalculateColor = new LiteralEnum<TextureCalculateColor>("TCCT");
        public LiteralInt TextureIdx = new LiteralInt("TxNo");
        public AVFXCurve Rate = new AVFXCurve( "Rate" );
        public AVFXCurve RPow = new AVFXCurve("RPow");

        List<Base> Attributes;

        public AVFXTextureReflection() : base("TR")
        {
            Attributes = new List<Base>(new Base[]{
                Enabled,
                UseScreenCopy,
                TextureFilter,
                TextureCalculateColor,
                TextureIdx,
                Rate,
                RPow
            });
        }

        public override void Read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void ToDefault()
        {
            Assigned = true;
            SetDefault(Attributes);
            SetUnAssigned(RPow);
            SetUnAssigned( Rate );
            TextureIdx.GiveValue( -1 );
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("TR");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
