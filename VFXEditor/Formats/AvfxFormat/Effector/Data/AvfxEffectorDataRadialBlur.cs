using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataRadialBlur : AvfxData {
        public readonly AvfxCurve Length = new( "Length", "Len" );
        public readonly AvfxCurve Strength = new( "Strength", "Str" );
        public readonly AvfxCurve Gradation = new( "Gradation", "Gra" );
        public readonly AvfxCurve InnerRadius = new( "Inner Radius", "IRad" );
        public readonly AvfxCurve OuterRadius = new( "Outer Radius", "ORad" );
        public readonly AvfxFloat FadeStartDistance = new( "Fade Start Distance", "FSDc" );
        public readonly AvfxFloat FadeEndDistance = new( "Fade End Distance", "FEDc" );
        public readonly AvfxEnum<ClipBasePoint> FadeBasePointType = new( "Fade Base Point", "FaBP" );

        public readonly UiDisplayList Display;

        public AvfxEffectorDataRadialBlur() : base() {
            Parsed = [
                Length,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            ];

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
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
