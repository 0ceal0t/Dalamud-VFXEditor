using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx
{
    public class UIParticleDataDecalRing : UIData {
        public AVFXParticleDataDecalRing Data;
        public UIParameters Parameters;

        public UIParticleDataDecalRing(AVFXParticleDataDecalRing data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UIFloat( "Scaling Scale", Data.ScalingScale ) );
            Parameters.Add( new UIFloat( "Ring Fan", Data.RingFan ) );
            Tabs.Add( new UICurve( Data.Width, "Width" ) );
        }
    }
}
