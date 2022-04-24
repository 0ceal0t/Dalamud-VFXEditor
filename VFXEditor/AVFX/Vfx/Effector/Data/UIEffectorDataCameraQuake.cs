using VFXEditor.AVFXLib.Effector;

namespace VFXEditor.AVFX.VFX {
    public class UIEffectorDataCameraQuake : UIData {
        public UIEffectorDataCameraQuake( AVFXEffectorDataCameraQuake data ) {
            Tabs.Add( new UICurve( data.Attenuation, "Attenuation" ) );
            Tabs.Add( new UICurve( data.RadiusOut, "Radius Out" ) );
            Tabs.Add( new UICurve( data.RadiusIn, "Radius In" ) );
            Tabs.Add( new UICurve3Axis( data.Rotation, "Rotation" ) );
            Tabs.Add( new UICurve3Axis( data.Position, "Position" ) );
        }
    }
}
