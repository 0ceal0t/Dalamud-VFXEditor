using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorDataCameraQuake : UiData {
        public UiEffectorDataCameraQuake( AVFXEffectorDataCameraQuake data ) {
            Tabs.Add( new UiCurve( data.Attenuation, "Attenuation" ) );
            Tabs.Add( new UiCurve( data.RadiusOut, "Radius Out" ) );
            Tabs.Add( new UiCurve( data.RadiusIn, "Radius In" ) );
            Tabs.Add( new UiCurve3Axis( data.Rotation, "Rotation" ) );
            Tabs.Add( new UiCurve3Axis( data.Position, "Position" ) );
        }
    }
}
