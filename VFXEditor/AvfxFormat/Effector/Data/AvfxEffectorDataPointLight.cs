using System;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
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

        public readonly UiDisplayList Display;

        public AvfxEffectorDataPointLight() : base() {
            Parsed = new() {
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

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( PointLightAttenuationType );
            Display.Add( EnableShadow );
            Display.Add( EnableCharShadow );
            Display.Add( EnableMapShadow );
            Display.Add( EnableMoveShadow );
            Display.Add( ShadowCreateDistanceNear );
            Display.Add( ShadowCreateDistanceFar );

            DisplayTabs.Add( Color );
            DisplayTabs.Add( DistanceScale ) ;
            DisplayTabs.Add( Rotation ) ;
            DisplayTabs.Add( Position ) ;
        }
    }
}
