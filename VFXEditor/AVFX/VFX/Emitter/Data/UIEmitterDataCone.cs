using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AVFX.VFX {
    public class UIEmitterDataCone : UIData {
        public readonly UIParameters Parameters;

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
