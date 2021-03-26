using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataCameraQuake : AVFXEffectorData
    {
        public AVFXCurve Attenuation = new AVFXCurve("Att");
        public AVFXCurve RadiusOut = new AVFXCurve("RdO");
        public AVFXCurve RadiusIn = new AVFXCurve("RdI");
        public AVFXCurve3Axis Rotation = new AVFXCurve3Axis("Rot");
        public AVFXCurve3Axis Position = new AVFXCurve3Axis("Pos");

        List<Base> Attributes;

        public AVFXEffectorDataCameraQuake() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
                Position
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

        public override AVFXNode toAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
