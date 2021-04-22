using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataLaser : UIData {
        public AVFXParticleDataLaser Data;

        public UIParticleDataLaser(AVFXParticleDataLaser data)
        {
            Data = data;
            //=======================
            Tabs.Add( new UICurve( Data.Width, "Width" ) );
            Tabs.Add( new UICurve( Data.Length, "Length" ) );
        }
    }
}
