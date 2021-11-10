using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIParticleDataPowder: UIData {
        public AVFXParticleDataPowder Data;
        public UIParameters Parameters;

        public UIParticleDataPowder(AVFXParticleDataPowder data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<DirectionalLightType>( "Directional Light Type", Data.DirectionalLightType ) );
            Parameters.Add( new UICheckbox( "Is Lightning", Data.IsLightning ) );
            Parameters.Add( new UIFloat( "Center Offset", Data.CenterOffset ) );
        }
    }
}
