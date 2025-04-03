using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataModelSkin : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxEnum<FresnelType> FresnelType = new( "Fresnel Type", "FrsT" );
        public readonly AvfxFlag<AuraFilter> AuraTarget = new( "Aura Target", "AuTT" );
        public readonly AvfxCurve1Axis FresnelCurve = new( "Fresnel Curve", "FrC" );
        public readonly AvfxCurve3Axis FresnelRotation = new( "Fresnel Rotation", "FrRt", CurveType.Angle );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );
        public readonly AvfxCurve1Axis SEM = new( "SEM", "SEM" );
        public readonly AvfxCurve1Axis EEM = new( "EEM", "EEM" );
        public readonly AvfxCurve3Axis UVPD = new( "UVPD", "UVPD" );

        public AvfxParticleDataModelSkin() : base() {
            Parsed = [
                FresnelType,
                AuraTarget,
                FresnelCurve,
                FresnelRotation,
                ColorBegin,
                ColorEnd,
                SEM,
                EEM,
                UVPD
            ];

            ParameterTab.Add( FresnelType );
            ParameterTab.Add( AuraTarget );

            Tabs.Add( FresnelCurve );
            Tabs.Add( FresnelRotation );
            Tabs.Add( ColorBegin );
            Tabs.Add( ColorEnd );
            Tabs.Add( SEM );
            Tabs.Add( EEM );
            Tabs.Add( UVPD );
        }
    }
}
