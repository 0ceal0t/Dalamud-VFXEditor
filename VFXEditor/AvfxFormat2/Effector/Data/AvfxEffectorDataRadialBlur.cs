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

        public readonly UiParameters Display;

        public AvfxEffectorDataRadialBlur() : base() {
            Parsed = new() {
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            };

            DisplayTabs.Add( Display = new UiParameters( "Parameters" ) );
            Display.Add( FadeStartDistance );
            Display.Add( FadeEndDistance );
            Display.Add( FadeBasePointType );

            DisplayTabs.Add( Length );
            DisplayTabs.Add( Strength );
            DisplayTabs.Add( Gradation );
            DisplayTabs.Add( InnerRadius );
            DisplayTabs.Add( OuterRadius );
        }
    }
}
