using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterDataCone : UiData {
        public readonly UiParameters Parameters;

        public UiEmitterDataCone( AVFXEmitterDataCone data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Tabs.Add( new UiCurve( data.AngleY, "Angle Y" ) );
            Tabs.Add( new UiCurve( data.OuterSize, "Outer Size" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
            Tabs.Add( new UiCurve( data.InjectionAngle, "Injection Angle" ) );
        }
    }
}
