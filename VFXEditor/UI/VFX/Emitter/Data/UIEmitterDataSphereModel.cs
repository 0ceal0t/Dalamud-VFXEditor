using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterDataSphereModel : UIData {
        public AVFXEmitterDataSphereModel Data;
        public UIParameters Parameters;

        public UIEmitterDataSphereModel(AVFXEmitterDataSphereModel data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(new UICombo<RotationOrder>("Rotation Order", Data.RotationOrderType));
            Parameters.Add(new UICombo<GenerateMethod>("Generate Method", Data.GenerateMethodType));
            Parameters.Add(new UIInt("Divide X", Data.DivideX));
            Parameters.Add(new UIInt("Divide Y", Data.DivideY));
            Tabs.Add(new UICurve(Data.Radius, "Radius"));
            Tabs.Add(new UICurve(Data.InjectionSpeed, "Injection Speed"));
            Tabs.Add( new UICurve( Data.InjectionSpeedRandom, "Injection Speed Random" ) );
        }
    }
}
