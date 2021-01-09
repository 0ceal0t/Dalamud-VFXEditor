using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataDirectionalLight : AVFXEffectorData
    {
        public AVFXCurveColor Ambient = new AVFXCurveColor( "ambient", "Amb" );
        public AVFXCurveColor Color = new AVFXCurveColor( "color", "Col" );
        public AVFXCurve Power = new AVFXCurve( "power", "Pow" );
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis( "rotation", "Rot" );

        List<Base> Attributes;

        public AVFXEffectorDataDirectionalLight(string jsonPath) : base(jsonPath, "Data")
        {
            Attributes = new List<Base>(new Base[]{
                Ambient,
                Color,
                Power,
                Rotation
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
            SetDefault(Attributes);
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
