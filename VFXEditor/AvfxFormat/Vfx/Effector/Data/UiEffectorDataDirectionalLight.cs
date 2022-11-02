using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorDataDirectionalLight : UiData {
        public UiEffectorDataDirectionalLight( AVFXEffectorDataDirectionalLight data ) {
            Tabs.Add( new UiCurveColor( data.Ambient, "Ambient" ) );
            Tabs.Add( new UiCurveColor( data.Color, "Color" ) );
            Tabs.Add( new UiCurve( data.Power, "Power" ) );
            Tabs.Add( new UiCurve3Axis( data.Rotation, "Rotation" ) );
        }
    }
}
