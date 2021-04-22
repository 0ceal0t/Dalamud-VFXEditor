using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterDataModel : UIData {
        public AVFXEmitterDataModel Data;
        public UIParameters Parameters;

        public UINodeSelect<UIModel> ModelSelect;

        public UIEmitterDataModel(AVFXEmitterDataModel data, UIEmitter emitter)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(ModelSelect = new UINodeSelect<UIModel>( emitter, "Model", UINode._Models, Data.ModelIdx ));
            Parameters.Add(new UICombo<RotationOrder>("Rotation Order", Data.RotationOrderType));
            Parameters.Add(new UICombo<GenerateMethod>("Generate Method", Data.GenerateMethodType));
            Tabs.Add(new UICurve(Data.AX, "Angle X"));
            Tabs.Add(new UICurve(Data.AY, "Angle Y"));
            Tabs.Add(new UICurve(Data.AZ, "Angle Z"));
            Tabs.Add(new UICurve(Data.InjectionSpeed, "Injection Speed"));
            Tabs.Add(new UICurve(Data.InjectionSpeedRandom, "Injection Speed Random"));
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
