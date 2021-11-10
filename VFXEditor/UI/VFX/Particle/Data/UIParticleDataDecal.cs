using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIParticleDataDecal : UIData {
        public AVFXParticleDataDecal Data;
        public UIParameters Parameters;

        public UIParticleDataDecal(AVFXParticleDataDecal data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(new UIFloat("Scaling Scale", Data.ScalingScale));
        }
    }
}
