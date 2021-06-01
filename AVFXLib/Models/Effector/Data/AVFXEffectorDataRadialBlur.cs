using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataRadialBlur : AVFXEffectorData
    {
        public AVFXCurve Length = new AVFXCurve("Len");
        public AVFXCurve Strength = new AVFXCurve("Str");
        public AVFXCurve Gradation = new AVFXCurve("Gra");
        public AVFXCurve InnerRadius = new AVFXCurve("IRad");
        public AVFXCurve OuterRadius = new AVFXCurve("ORad");
        public LiteralFloat FadeStartDistance = new LiteralFloat( "FSDc" );
        public LiteralFloat FadeEndDistance = new LiteralFloat( "FEDc" );
        public LiteralEnum<ClipBasePoint> FadeBasePointType = new LiteralEnum<ClipBasePoint>( "FaBP" );

        List<Base> Attributes;

        public AVFXEffectorDataRadialBlur() : base("Data")
        {
            Attributes = new List<Base>(new Base[]{
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            } );
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
            SetUnAssigned(Length);
            SetUnAssigned(Strength);
            SetUnAssigned(Gradation);
            SetUnAssigned(InnerRadius);
            SetUnAssigned(OuterRadius);
        }

        public override AVFXNode ToAVFX()
        {
            AVFXNode dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
