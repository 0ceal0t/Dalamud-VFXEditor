using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterDataCylinderModel : UiData {
        public readonly UiParameters Parameters;

        public UiEmitterDataCylinderModel( AVFXEmitterDataCylinderModel data ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( new UiCombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Parameters.Add( new UiCombo<GenerateMethod>( "Generate Method", data.GenerateMethodType ) );
            Parameters.Add( new UiInt( "Divide X", data.DivideX ) );
            Parameters.Add( new UiInt( "Divide Y", data.DivideY ) );
            Tabs.Add( new UiCurve( data.Radius, "Radius" ) );
            Tabs.Add( new UiCurve( data.Length, "Length" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
        }
    }
}
