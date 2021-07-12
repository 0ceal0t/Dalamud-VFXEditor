using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataDisc : UIData {
        public AVFXParticleDataDisc Data;
        public UIParameters Parameters;

        public UIParticleDataDisc(AVFXParticleDataDisc data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIInt( "Parts Count", Data.PartsCount ) );
            Parameters.Add( new UIInt( "Parts Count U", Data.PartsCountU ) );
            Parameters.Add( new UIInt( "Parts Count V", Data.PartsCountV ) );
            Parameters.Add( new UIFloat( "Point Interval Factor V", Data.PointIntervalFactoryV ) );
            Tabs.Add( new UICurve( Data.Angle, "Angle" ) );
            Tabs.Add( new UICurve( Data.HBInner, "HB Inner" ) );
            Tabs.Add( new UICurve( Data.HEInner, "HE Inner" ) );
            Tabs.Add( new UICurve( Data.HBOuter, "HB Outer" ) );
            Tabs.Add( new UICurve( Data.HEOuter, "HE Outer" ) );
            Tabs.Add( new UICurve( Data.WidthBegin, "Width Begin" ) );
            Tabs.Add( new UICurve( Data.WidthEnd, "Width End" ) );
            Tabs.Add( new UICurve( Data.RadiusBegin, "Radius Begin" ) );
            Tabs.Add( new UICurve( Data.RadiusEnd, "Radius End" ) );
            Tabs.Add( new UICurveColor( Data.ColorEdgeInner, "Color Edge Inner" ) );
            Tabs.Add( new UICurveColor( Data.ColorEdgeOuter, "Color Edge Outer" ) );
        }
    }
}
