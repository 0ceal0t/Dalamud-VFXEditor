using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterDataModel : UiData {
        public readonly UiParameters Parameters;
        public UiNodeSelect<UiModel> ModelSelect;

        public UiEmitterDataModel( AVFXEmitterDataModel data, UiEmitter emitter ) {
            Tabs.Add( Parameters = new UiParameters( "Parameters" ) );
            Parameters.Add( ModelSelect = new UiNodeSelect<UiModel>( emitter, "Model", emitter.NodeGroups.Models, data.ModelIdx ) );
            Parameters.Add( new UiCombo<RotationOrder>( "Rotation Order", data.RotationOrderType ) );
            Parameters.Add( new UiCombo<GenerateMethod>( "Generate Method", data.GenerateMethodType ) );
            Tabs.Add( new UiCurve( data.AX, "Angle X" ) );
            Tabs.Add( new UiCurve( data.AY, "Angle Y" ) );
            Tabs.Add( new UiCurve( data.AZ, "Angle Z" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UiCurve( data.InjectionSpeedRandom, "Injection Speed Random" ) );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
