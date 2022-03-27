using AVFXLib.Models;

namespace VFXEditor.Avfx.Vfx {
    public class UIEffectorDataDirectionalLight : UIData {
        public UIEffectorDataDirectionalLight( AVFXEffectorDataDirectionalLight data ) {
            Tabs.Add( new UICurveColor( data.Ambient, "Ambient" ) );
            Tabs.Add( new UICurveColor( data.Color, "Color" ) );
            Tabs.Add( new UICurve( data.Power, "Power" ) );
            Tabs.Add( new UICurve3Axis( data.Rotation, "Rotation" ) );
        }
    }
}
