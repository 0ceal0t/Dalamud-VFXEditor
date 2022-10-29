using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AVFX.VFX {
    public class UIEmitterDataConeModel : UIData {
        public readonly UIParameters Parameters;

        public UIEmitterDataConeModel( AVFXEmitterDataConeModel data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Parameters.Add( new UICombo<GenerateMethod>( "Generate Method", data.GenerateMethodType ) );
            Parameters.Add( new UIInt( "Divide X", data.DivideX ) );
            Parameters.Add( new UIInt( "Divide Y", data.DivideY ) );
            Tabs.Add( new UICurve( data.AX, "Angle X" ) );
            Tabs.Add( new UICurve( data.AY, "Angle Y" ) );
            Tabs.Add( new UICurve( data.Radius, "Radius" ) );
            Tabs.Add( new UICurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UICurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
            Tabs.Add( new UICurve( data.InjectionAngle, "Injection Angle" ) );
        }
    }
}
