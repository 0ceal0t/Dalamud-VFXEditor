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
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UINodeSelect<UIModel> ModelSelect;

        public UIEmitterDataModel(AVFXEmitterDataModel data, UIEmitter emitter)
        {
            Data = data;
            //=======================
            ModelSelect = new UINodeSelect<UIModel>( emitter, "Model", UINode._Models, Data.ModelIdx );

            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Data.RotationOrderType));
            Attributes.Add(new UICombo<GenerateMethod>("Generate Method", Data.GenerateMethodType));
            Attributes.Add(new UICurve(Data.AX, "Angle X"));
            Attributes.Add(new UICurve(Data.AY, "Angle Y"));
            Attributes.Add(new UICurve(Data.AZ, "Angle Z"));
            Attributes.Add(new UICurve(Data.InjectionSpeed, "Injection Speed"));
            Attributes.Add(new UICurve(Data.InjectionSpeedRandom, "Injection Speed Random"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
