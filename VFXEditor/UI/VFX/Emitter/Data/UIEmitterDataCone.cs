using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIEmitterDataCone : UIData
    {
        public AVFXEmitterDataCone Data;
        public UIParameters Parameters;

        public UIEmitterDataCone( AVFXEmitterDataCone data ) {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<RotationOrder>( "Rotation Order", Data.RotationOrderType ) );
            Tabs.Add( new UICurve( Data.AngleY, "Angle Y" ) );
            Tabs.Add( new UICurve( Data.OuterSize, "Outer Size" ) );
            Tabs.Add( new UICurve( Data.InjectionSpeed, "Injection Speed" ) );
            Tabs.Add( new UICurve( Data.InjectionSpeedRandom, "Injection Speed Random" ) );
            Tabs.Add( new UICurve( Data.InjectionAngle, "Injection Angle" ) );
        }
    }
}
