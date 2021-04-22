using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataPowder: UIData {
        public AVFXParticleDataPowder Data;

        public UIParticleDataPowder(AVFXParticleDataPowder data)
        {
            Data = data;
            //=======================
            Attributes.Add( new UICombo<DirectionalLightType>( "Directional Light Type", Data.DirectionalLightType ) );
            Attributes.Add( new UICheckbox( "Is Lightning", Data.IsLightning ) );
            Attributes.Add( new UIFloat( "Center Offset", Data.CenterOffset ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
