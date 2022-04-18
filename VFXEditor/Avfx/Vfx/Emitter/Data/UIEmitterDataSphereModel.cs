using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterDataSphereModel : UIData {
        public UIParameters Parameters;

        public UIEmitterDataSphereModel( AVFXEmitterDataSphereModel data ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Parameters.Add( new UICombo<GenerateMethod>( "Generate Method", data.GenerateMethodType ) );
            Parameters.Add( new UIInt( "Divide X", data.DivideX ) );
            Parameters.Add( new UIInt( "Divide Y", data.DivideY ) );
            Tabs.Add( new UICurve( data.Radius, "Radius" ) );
            Tabs.Add( new UICurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UICurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
        }
    }
}
