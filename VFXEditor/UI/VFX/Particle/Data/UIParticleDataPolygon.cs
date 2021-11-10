using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIParticleDataPolygon : UIData {
        public AVFXParticleDataPolygon Data;

        public UIParticleDataPolygon(AVFXParticleDataPolygon data)
        {
            Data = data;
            //=======================
            Tabs.Add( new UICurve( Data.Count, "Count" ) );
        }
    }
}
