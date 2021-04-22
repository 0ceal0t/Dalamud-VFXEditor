using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorDataDirectionalLight : UIData {
        public AVFXEffectorDataDirectionalLight Data;

        public UIEffectorDataDirectionalLight( AVFXEffectorDataDirectionalLight data )
        {
            Data = data;
            //=======================
            Tabs.Add( new UICurveColor( Data.Ambient, "Ambient" ) );
            Tabs.Add( new UICurveColor( Data.Color, "Color" ) );
            Tabs.Add( new UICurve( Data.Power, "Power" ) );
            Tabs.Add( new UICurve3Axis( Data.Rotation, "Rotation" ) );
        }
    }
}
