using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataModel : UIData {
        public AVFXParticleDataModel Data;
        public UINodeSelectList<UIModel> ModelSelect;
        public UIParameters Parameters;
        //==========================

        public UIParticleDataModel(AVFXParticleDataModel data, UIParticle particle)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add(ModelSelect = new UINodeSelectList<UIModel>( particle, "Model", UINode._Models, Data.ModelIdx ));
            Parameters.Add( new UIInt( "Model Number Random", Data.ModelNumberRandomValue ) );
            Parameters.Add( new UICombo<RandomType>( "Model Number Random Type", Data.ModelNumberRandomType ) );
            Parameters.Add( new UIInt( "Model Number Random Interval", Data.ModelNumberRandomInterval ) );
            Parameters.Add( new UICombo<FresnelType>( "Fresnel Type", Data.FresnelType ) );
            Parameters.Add( new UICombo<DirectionalLightType>( "Directional Light Type", Data.DirectionalLightType ) );
            Parameters.Add( new UICombo<PointLightType>( "Point Light Type", Data.PointLightType ) );
            Parameters.Add( new UICheckbox( "Is Lightning", Data.IsLightning ) );
            Parameters.Add( new UICheckbox( "Is Morph", Data.IsMorph ) );

            Tabs.Add( new UICurve( Data.Morph, "Morph" ) );
            Tabs.Add( new UICurve( Data.FresnelCurve, "Fresnel Curve" ) );
            Tabs.Add( new UICurve3Axis( Data.FresnelRotation, "Fresnel Rotation" ) );
            Tabs.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Tabs.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
        }

        public override void Dispose() {
            ModelSelect.DeleteSelect();
        }
    }
}
