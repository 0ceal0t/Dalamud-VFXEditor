using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXParticleDataLaser : AVFXParticleData
    {
        public AVFXCurve Width = new AVFXCurve("width", "Wd");
        public AVFXCurve Length = new AVFXCurve("length", "Len");


        List<Base> Attributes;

        public AVFXParticleDataLaser(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                Width,
                Length
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
            SetUnAssigned(Attributes);
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
