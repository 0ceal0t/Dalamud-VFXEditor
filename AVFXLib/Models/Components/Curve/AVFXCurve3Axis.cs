using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXCurve3Axis : Base
    {
        public LiteralEnum<AxisConnect> AxisConnectType = new LiteralEnum<AxisConnect>("ACT");
        public LiteralEnum<RandomType> AxisConnectRandomType = new LiteralEnum<RandomType>("ACTR");
        public AVFXCurve X = new AVFXCurve("X");
        public AVFXCurve Y = new AVFXCurve("Y");
        public AVFXCurve Z = new AVFXCurve("Z");
        public AVFXCurve RX = new AVFXCurve("XR");
        public AVFXCurve RY = new AVFXCurve("YR");
        public AVFXCurve RZ = new AVFXCurve("ZR");

        List<Base> Attributes;

        public AVFXCurve3Axis(string avfxName) : base(avfxName)
        {
            Attributes = new List<Base>(new Base[]{
                AxisConnectType,
                AxisConnectRandomType,
                X,
                Y,
                Z,
                RX,
                RY,
                RZ
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
            SetUnAssigned(Attributes);
            SetDefault(AxisConnectType);
            SetDefault(AxisConnectRandomType);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode curveAvfx = new AVFXNode(AVFXName);
            PutAVFX(curveAvfx, Attributes);
            return curveAvfx;
        }
    }
}
