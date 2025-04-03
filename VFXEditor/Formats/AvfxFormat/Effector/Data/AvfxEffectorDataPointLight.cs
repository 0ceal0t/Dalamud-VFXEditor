using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataPointLight : AvfxDataWithParameters {
        public readonly AvfxCurveColor Color = new( "Color" );
        public readonly AvfxCurve1Axis DistanceScale = new( "Distance Scale", "DstS" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos" );
        public readonly AvfxEnum<PointLightAttenuation> PointLightAttenuationType = new( "Point Light Attenuation", "Attn" );
        public readonly AvfxBool EnableShadow = new( "Enable Shadow", "bSdw" );
        public readonly AvfxBool EnableCharShadow = new( "Enable Char Shadow", "bChS" );
        public readonly AvfxBool EnableMapShadow = new( "Enable Map Shadow", "bMpS" );
        public readonly AvfxBool EnableMoveShadow = new( "Enabled Move Shadow", "bMvS" );
        public readonly AvfxFloat ShadowCreateDistanceNear = new( "Create Distance Near", "SCDN" );
        public readonly AvfxFloat ShadowCreateDistanceFar = new( "Create Distance Far", "SCDF" );

        public AvfxEffectorDataPointLight() : base() {
            Parsed = [
                Color,
                DistanceScale,
                Rotation,
                Position,
                PointLightAttenuationType,
                EnableShadow,
                EnableCharShadow,
                EnableMapShadow,
                EnableMoveShadow,
                ShadowCreateDistanceNear,
                ShadowCreateDistanceFar
            ];

            ParameterTab.Add( PointLightAttenuationType );
            ParameterTab.Add( EnableShadow );
            ParameterTab.Add( EnableCharShadow );
            ParameterTab.Add( EnableMapShadow );
            ParameterTab.Add( EnableMoveShadow );
            ParameterTab.Add( ShadowCreateDistanceNear );
            ParameterTab.Add( ShadowCreateDistanceFar );

            Tabs.Add( Color );
            Tabs.Add( DistanceScale );
            Tabs.Add( Rotation );
            Tabs.Add( Position );
        }
    }
}
