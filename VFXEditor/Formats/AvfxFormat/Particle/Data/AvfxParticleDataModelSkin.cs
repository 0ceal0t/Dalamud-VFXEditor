using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Bool;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using VfxEditor.Formats.AvfxFormat.Components.Flag;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataModelSkin : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxEnum<FresnelType> FresnelType = new( "Fresnel Type", "FrsT" );
        public readonly AvfxFlag<AuraFilter> AuraTarget = new( "Aura Target", "AuTT" );
        public readonly AvfxBool CM = new( "CM", "bCM" );
        public readonly AvfxCurve1Axis FresnelCurve = new( "Fresnel Curve", "FrC" );
        public readonly AvfxCurve1Axis FresnelCurveRandom = new( "Fresnel Curve Random", "FrCR" );
        public readonly AvfxCurve3Axis FresnelRotation = new( "Fresnel Rotation", "FrRt", CurveType.Angle );
        public readonly AvfxCurveColor ColorBegin;
        public readonly AvfxCurveColor ColorEnd;
        public readonly AvfxCurve1Axis SEM = new( "Specular Mask", "SEM" );
        public readonly AvfxCurve1Axis SEMRandom = new( "Specular Random", "SEMR" );
        public readonly AvfxCurve1Axis EEM = new( "Emissive Mask", "EEM" );
        public readonly AvfxCurve1Axis EEMRandom = new( "Emissive Random", "EEMR" );
        public readonly AvfxCurve3Axis UVPD = new( "UV Point Density", "UVPD" );

        public AvfxParticleDataModelSkin( AvfxFile file ) : base() {
            ColorBegin = new( file, name: "Color Begin", "ColB" );
            ColorEnd = new( file, name: "Color End", "ColE" );

            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleDataModelSkin ) );

            ParameterTab.Add( FresnelType );
            ParameterTab.Add( AuraTarget );
            ParameterTab.Add( CM );

            Tabs.Add( FresnelCurve );
            Tabs.Add( FresnelCurveRandom );
            Tabs.Add( FresnelRotation );
            Tabs.Add( ColorBegin );
            Tabs.Add( ColorEnd );
            Tabs.Add( SEM );
            Tabs.Add( SEMRandom );
            Tabs.Add( EEM );
            Tabs.Add( EEMRandom );
            Tabs.Add( UVPD );
        }
    }
}
