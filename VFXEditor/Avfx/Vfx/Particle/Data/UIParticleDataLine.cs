using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx
{
    public class UIParticleDataLine : UIData {
        public AVFXParticleDataLine Data;
        public UIParameters Parameters;

        public UIParticleDataLine( AVFXParticleDataLine data )
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIInt( "Line Count", Data.LineCount ) );
            Tabs.Add( new UICurve( Data.Length, "Length" ) );
            Tabs.Add( new UICurve( Data.LengthRandom, "Length Random" ) );
            Tabs.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
        }
    }
}
