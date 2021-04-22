using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorDataRadialBlur : UIData {
        public AVFXEffectorDataRadialBlur Data;
        public UIParameters Parameters;

        public UIEffectorDataRadialBlur(AVFXEffectorDataRadialBlur data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(new UIFloat("Fade Start Distance", Data.FadeStartDistance));
            Parameters.Add(new UIFloat("Fade End Distance", Data.FadeEndDistance));
            Parameters.Add(new UICombo<ClipBasePoint>("Fade Base Point", Data.FadeBasePointType));

            Tabs.Add(new UICurve(Data.Length, "Length"));
            Tabs.Add(new UICurve(Data.Strength, "Strength"));
            Tabs.Add(new UICurve(Data.Gradation, "Gradation"));
            Tabs.Add(new UICurve(Data.InnerRadius, "Inner Radius"));
            Tabs.Add(new UICurve(Data.OuterRadius, "Outer Radius"));
        }
    }
}
