using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEffectorDataRadialBlur : AVFXEffectorData
    {
        public AVFXCurve Length = new("Len");
        public AVFXCurve Strength = new("Str");
        public AVFXCurve Gradation = new("Gra");
        public AVFXCurve InnerRadius = new("IRad");
        public AVFXCurve OuterRadius = new("ORad");
        public LiteralFloat FadeStartDistance = new( "FSDc" );
        public LiteralFloat FadeEndDistance = new( "FEDc" );
        public LiteralEnum<ClipBasePoint> FadeBasePointType = new( "FaBP" );
        readonly List<Base> Attributes;

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
            var dataAvfx = new AVFXNode("Data");
            PutAVFX(dataAvfx, Attributes);
            return dataAvfx;
        }
    }
}
