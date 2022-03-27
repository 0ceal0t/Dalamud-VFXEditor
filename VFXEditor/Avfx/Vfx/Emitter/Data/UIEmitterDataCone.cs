using AVFXLib.Models;

namespace VFXEditor.Avfx.Vfx {
    public class UIEmitterDataCone : UIData {
        public UIParameters Parameters;

        public UIEmitterDataCone( AVFXEmitterDataCone data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Tabs.Add( new UICurve( data.AngleY, "Angle Y" ) );
            Tabs.Add( new UICurve( data.OuterSize, "Outer Size" ) );
            Tabs.Add( new UICurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UICurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
            Tabs.Add( new UICurve( data.InjectionAngle, "Injection Angle" ) );
        }
    }
}
