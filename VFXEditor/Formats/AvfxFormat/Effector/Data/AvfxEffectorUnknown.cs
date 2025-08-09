using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorUnknown : AvfxDataWithParameters {

        public readonly AvfxCurve1Axis CSR = new( "CSR", "CSR" );
        public readonly AvfxCurve1Axis CSS = new( "CSS", "CSS" );
        public readonly AvfxCurve1Axis CGE = new( "CGE", "CGE" );
        public readonly AvfxCurve1Axis Strength = new( "Strength", "Str" );
        public readonly AvfxCurve1Axis Gradation = new( "Gradation", "Gra" );
        public readonly AvfxCurve1Axis InnerRadius = new( "Inner Radius", "IRad" );
        public readonly AvfxCurve1Axis OuterRadius = new( "Outer Radius", "ORad" );
        public readonly AvfxFloat FadeStartDistance = new( "Fade Start Distance", "FSDc" );
        public readonly AvfxFloat FadeEndDistance = new( "Fade End Distance", "FEDc" );
        public readonly AvfxEnum<ClipBasePoint> FadeBasePointType = new( "Fade Base Point", "FaBP" );


        public AvfxEffectorUnknown() : base() {
            Parsed = [
                CSR,
                CSS,
                CGE,
                Strength,
                Gradation,
                InnerRadius,
                OuterRadius,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType
            ];

            ParameterTab.Add( FadeStartDistance );
            ParameterTab.Add( FadeEndDistance );
            ParameterTab.Add( FadeBasePointType );

            Tabs.Add( CSR );
            Tabs.Add( CSS );
            Tabs.Add( CGE );
            Tabs.Add( Strength );
            Tabs.Add( Gradation );
            Tabs.Add( InnerRadius );
            Tabs.Add( OuterRadius );

        }
    }
}
