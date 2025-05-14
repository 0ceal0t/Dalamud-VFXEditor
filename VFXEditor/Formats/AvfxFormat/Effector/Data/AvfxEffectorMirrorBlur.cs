using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorMirrorBlur : AvfxDataWithParameters {
        public readonly AvfxCurve1Axis Length = new( "Length", "Len" );
        public readonly AvfxCurve1Axis Strength = new( "Strength", "Str" );
        public readonly AvfxCurve1Axis AStrength = new( "Angle Strength", "AStr" );
        public readonly AvfxCurve1Axis Angle = new( "Angle", "Ang" );
        public readonly AvfxFloat FadeStartDistance = new( "Fade Start Distance", "FSDc" );
        public readonly AvfxFloat FadeEndDistance = new( "Fade End Distance", "FEDc" );
        public readonly AvfxEnum<ClipBasePoint> FadeBasePointType = new( "Fade Base Point", "FaBP" );
        public readonly AvfxBool OneSide = new( "bOS", "Single Side" );


        public AvfxEffectorMirrorBlur() : base() {
            Parsed = [
                Length,
                Strength,
                AStrength,
                Angle,
                FadeStartDistance,
                FadeEndDistance,
                FadeBasePointType,
                OneSide
            ];

            ParameterTab.Add( FadeStartDistance );
            ParameterTab.Add( FadeEndDistance );
            ParameterTab.Add( FadeBasePointType );
            ParameterTab.Add( OneSide );

            Tabs.Add( Length );
            Tabs.Add( Strength );
            Tabs.Add( AStrength );
            Tabs.Add( Angle );

        }
    }
}
