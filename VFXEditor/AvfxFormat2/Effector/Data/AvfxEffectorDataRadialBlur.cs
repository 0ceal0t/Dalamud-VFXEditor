using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEffectorDataRadialBlur : AvfxData {
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve Strength = new( "Strength", "Str" );
        public readonly AvfxCurve Gradation = new( "Gradation", "Gra" );
        public readonly AvfxCurve InnerRadius = new( "Inner Radius", "IRad" );
        public readonly AvfxCurve OuterRadius = new( "Outer Radius", "ORad" );
        public readonly AvfxFloat FadeStartDistance = new( "Fade Start Distance", "FSDc" );
        public readonly AvfxFloat FadeEndDistance = new( "Fade End Distance", "FEDc" );
        public readonly AvfxEnum<ClipBasePoint> FadeBasePointType = new( "Fade Base Point", "FaBP" );

        public readonly UiParameters Parameters;

        public AvfxEffectorDataRadialBlur() : base() {
            Children = new() {
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( FadeStartDistance );
            Parameters.Add( FadeEndDistance );
            Parameters.Add( FadeBasePointType );

            Tabs.Add( Length );
            Tabs.Add( Strength );
            Tabs.Add( Gradation );
            Tabs.Add( InnerRadius );
            Tabs.Add( OuterRadius );
        }
    }
}
