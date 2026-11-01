using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Bool;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using VfxEditor.Formats.AvfxFormat.Components.Float;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Effector.Data {
    public class AvfxEffectorDataDirectionalBlur : AvfxDataWithParameters {
        public readonly AvfxCurve1Axis Length = new( "Length", "Len" );
        public readonly AvfxCurve1Axis Strength = new( "Strength", "Str" );
        public readonly AvfxCurve1Axis AngleStrength = new( "Angle Strength", "AStr" );
        public readonly AvfxCurve1Axis Angle = new( "Angle", "Ang" );
        public readonly AvfxFloat FadeStartDistance = new( "Fade Start Distance", "FSDc" );
        public readonly AvfxFloat FadeEndDistance = new( "Fade End Distance", "FEDc" );
        public readonly AvfxEnum<ClipBasePoint> FadeBasePointType = new( "Fade Base Point", "FaBP" );
        public readonly AvfxBool OneSide = new( "Single Side", "bOS" );


        public AvfxEffectorDataDirectionalBlur() : base() {
            Parsed = GetFieldsInOrder<AvfxBase>();

            ParameterTab.Add( FadeStartDistance );
            ParameterTab.Add( FadeEndDistance );
            ParameterTab.Add( FadeBasePointType );
            ParameterTab.Add( OneSide );

            Tabs.Add( Length );
            Tabs.Add( Strength );
            Tabs.Add( AngleStrength );
            Tabs.Add( Angle );

        }
    }
}
