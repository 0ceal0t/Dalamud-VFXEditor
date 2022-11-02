using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorDataPointLight : UiData {
        public UiParameters Parameters;

        public UiEffectorDataPointLight( AVFXEffectorDataPointLight data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<PointLightAttenuation>( "Point Light Attenuation", data.PointLightAttenuationType ) );
            Parameters.Add( new UiCheckbox( "Enable Shadow", data.EnableShadow ) );
            Parameters.Add( new UiCheckbox( "Enable Char Shadow", data.EnableCharShadow ) );
            Parameters.Add( new UiCheckbox( "Enable Map Shadow", data.EnableMapShadow ) );
            Parameters.Add( new UiCheckbox( "Enabled Move Shadow", data.EnableMoveShadow ) );
            Parameters.Add( new UiFloat( "Create Distance Near", data.ShadowCreateDistanceNear ) );
            Parameters.Add( new UiFloat( "Create Distance Far", data.ShadowCreateDistanceFar ) );

            Tabs.Add( new UiCurveColor( data.Color, "Color" ) );
            Tabs.Add( new UiCurve( data.DistanceScale, "Distance Scale" ) );
            Tabs.Add( new UiCurve3Axis( data.Rotation, "Rotation" ) );
            Tabs.Add( new UiCurve3Axis( data.Position, "Position" ) );
        }
    }
}
