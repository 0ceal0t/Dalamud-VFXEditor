using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Effector;

namespace VFXEditor.AVFX.VFX {
    public class UIEffectorDataPointLight : UIData {
        public UIParameters Parameters;

        public UIEffectorDataPointLight( AVFXEffectorDataPointLight data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<PointLightAttenuation>( "Point Light Attenuation", data.PointLightAttenuationType ) );
            Parameters.Add( new UICheckbox( "Enable Shadow", data.EnableShadow ) );
            Parameters.Add( new UICheckbox( "Enable Char Shadow", data.EnableCharShadow ) );
            Parameters.Add( new UICheckbox( "Enable Map Shadow", data.EnableMapShadow ) );
            Parameters.Add( new UICheckbox( "Enabled Move Shadow", data.EnableMoveShadow ) );
            Parameters.Add( new UIFloat( "Create Distance Near", data.ShadowCreateDistanceNear ) );
            Parameters.Add( new UIFloat( "Create Distance Far", data.ShadowCreateDistanceFar ) );

            Tabs.Add( new UICurveColor( data.Color, "Color" ) );
            Tabs.Add( new UICurve( data.DistanceScale, "Distance Scale" ) );
            Tabs.Add( new UICurve3Axis( data.Rotation, "Rotation" ) );
            Tabs.Add( new UICurve3Axis( data.Position, "Position" ) );
        }
    }
}
