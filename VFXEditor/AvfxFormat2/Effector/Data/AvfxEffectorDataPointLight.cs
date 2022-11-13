using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxEffectorDataPointLight : AvfxData {
        public readonly AvfxCurveColor Color = new( "Color" );
        public readonly AvfxCurve DistanceScale = new( "Distance Scale", "DstS" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos" );
        public readonly AvfxEnum<PointLightAttenuation> PointLightAttenuationType = new( "Point Light Attenuation", "Attn" );
        public readonly AvfxBool EnableShadow = new( "Enable Shadow", "bSdw" );
        public readonly AvfxBool EnableCharShadow = new( "Enable Char Shadow", "bChS" );
        public readonly AvfxBool EnableMapShadow = new( "Enable Map Shadow", "bMpS" );
        public readonly AvfxBool EnableMoveShadow = new( "Enabled Move Shadow", "bMvS" );
        public readonly AvfxFloat ShadowCreateDistanceNear = new( "Create Distance Near", "SCDN" );
        public readonly AvfxFloat ShadowCreateDistanceFar = new( "Create Distance Far", "SCDF" );

        public readonly UiParameters Parameters;

        public AvfxEffectorDataPointLight() : base() {
            Children = new() {
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
            };

            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( PointLightAttenuationType );
            Parameters.Add( EnableShadow );
            Parameters.Add( EnableCharShadow );
            Parameters.Add( EnableMapShadow );
            Parameters.Add( EnableMoveShadow );
            Parameters.Add( ShadowCreateDistanceNear );
            Parameters.Add( ShadowCreateDistanceFar );

            Tabs.Add( Color );
            Tabs.Add( DistanceScale ) ;
            Tabs.Add( Rotation ) ;
            Tabs.Add( Position ) ;
        }
    }
}
