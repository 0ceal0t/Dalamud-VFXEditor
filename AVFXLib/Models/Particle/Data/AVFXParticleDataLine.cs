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
        public LiteralInt LineCount = new LiteralInt( "lineCount", "LnCT" );
        public AVFXCurve Length = new AVFXCurve( "length", "Len" );
        public AVFXCurveColor ColorBegin = new AVFXCurveColor( "colorBegin", name: "ColB" );
        public AVFXCurveColor ColorEnd = new AVFXCurveColor( "colorEnd", name: "ColE" );

        List<Base> Attributes;

        public AVFXParticleDataLine(string jsonPath) : base(jsonPath, "Data")
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

        public override JToken toJSON()
        {
            JObject elem = new JObject();
            PutJSON(elem, Attributes);
            return elem;
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
