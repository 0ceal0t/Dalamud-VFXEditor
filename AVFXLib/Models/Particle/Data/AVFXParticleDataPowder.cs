using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataPowder : AVFXParticleData
    {
        public LiteralBool IsLightning = new("bLgt");
        public LiteralEnum<DirectionalLightType> DirectionalLightType = new("LgtT");
        public LiteralFloat CenterOffset = new("CnOf");
        readonly List<Base> Attributes;

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
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
