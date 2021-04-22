using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataLightModel : UIData {
        public AVFXParticleDataLightModel Data;
        public UINodeSelect<UIModel> ModelSelect;
        public UIParameters Parameters;

        public UIParticleDataLightModel(AVFXParticleDataLightModel data, UIParticle particle)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(ModelSelect = new UINodeSelect<UIModel>( particle, "Model", UINode._Models, Data.ModelIdx ));
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
