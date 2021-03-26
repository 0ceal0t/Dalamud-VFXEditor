using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataLine : AVFXParticleData
    {
        public LiteralInt LineCount = new LiteralInt( "LnCT" );
        public AVFXCurve Length = new AVFXCurve( "Len" );
        public AVFXCurveColor ColorBegin = new AVFXCurveColor(name: "ColB" );
        public AVFXCurveColor ColorEnd = new AVFXCurveColor(name: "ColE" );

        List<Base> Attributes;

        public AVFXParticleDataLine() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                LineCount,
                Length,
                ColorBegin,
                ColorEnd
            });
        }

        public override void read(AVFXNode node)
        {
            Assigned = true;
            ReadAVFX(Attributes, node);
        }

        public override void toDefault()
        {
            Assigned = true;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
