using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataPowder : AVFXParticleData
    {
        public LiteralBool IsLightning = new LiteralBool("bLgt");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new LiteralEnum<DirectionalLightType>("LgtT");
        public LiteralFloat CenterOffset = new LiteralFloat("CnOf");

        List<Base> Attributes;

        public AVFXParticleDataPowder() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                IsLightning,
                DirectionalLightType,
                CenterOffset
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
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
