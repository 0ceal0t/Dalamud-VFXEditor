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
        //==========================
        public UINodeSelect<UIModel> ModelSelect;

        public UIParticleDataLightModel(AVFXParticleDataLightModel data, UIParticle particle)
        {
            Data = data;
            //=======================
            ModelSelect = new UINodeSelect<UIModel>( particle, "Model", UINode._Models, Data.ModelIdx );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            ModelSelect.Draw( id );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
