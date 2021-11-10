using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIEffectorDataCameraQuake : UIData {
        public AVFXEffectorDataCameraQuake Data;

        public UIEffectorDataCameraQuake(AVFXEffectorDataCameraQuake data)
        {
            Data = data;
            //=======================
            Tabs.Add(new UICurve(Data.Attenuation, "Attenuation"));
            Tabs.Add(new UICurve(Data.RadiusOut, "Radius Out"));
            Tabs.Add(new UICurve(Data.RadiusIn, "Radius In"));
            Tabs.Add(new UICurve3Axis(Data.Rotation, "Rotation"));
            Tabs.Add(new UICurve3Axis(Data.Position, "Position"));
        }
    }
}
