using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterDataModel : UIData {
        public readonly UIParameters Parameters;
        public UINodeSelect<UIModel> ModelSelect;

        public UIEmitterDataModel( AVFXEmitterDataModel data, UIEmitter emitter ) {
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UINodeSelect<UIModel>( emitter, "Model", emitter.NodeGroups.Models, data.ModelIdx ) );
            Parameters.Add( new UICombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Parameters.Add( new UICombo<GenerateMethod>( "Generate Method", data.GenerateMethodType ) );
            Tabs.Add( new UICurve( data.AX, "Angle X" ) );
            Tabs.Add( new UICurve( data.AY, "Angle Y" ) );
            Tabs.Add( new UICurve( data.AZ, "Angle Z" ) );
            Tabs.Add( new UICurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UICurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
