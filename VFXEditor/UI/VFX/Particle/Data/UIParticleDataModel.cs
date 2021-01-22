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
        public List<UIBase> Attributes = new List<UIBase>();
        public UINodeSelectList<UIModel> ModelSelect;
        //==========================

        public UIParticleDataModel(AVFXParticleDataModel data, UIParticle particle)
        {
            Data = data;
            //=======================
            ModelSelect = new UINodeSelectList<UIModel>( particle, "Model", UINode._Models, Data.ModelIdx );

            Attributes.Add( new UIInt( "Model Number Random", Data.ModelNumberRandomValue ) );
            Attributes.Add( new UICombo<RandomType>( "Model Number Random Type", Data.ModelNumberRandomType ) );
            Attributes.Add( new UIInt( "Model Number Random Interval", Data.ModelNumberRandomInterval ) );
            Attributes.Add( new UICombo<FresnelType>( "Fresnel Type", Data.FresnelType ) );
            Attributes.Add( new UICombo<DirectionalLightType>( "Directional Light Type", Data.DirectionalLightType ) );
            Attributes.Add( new UICombo<PointLightType>( "Point Light Type", Data.PointLightType ) );
            Attributes.Add( new UICheckbox( "Is Lightning", Data.IsLightning ) );
            Attributes.Add( new UICheckbox( "Is Morph", Data.IsMorph ) );
            Attributes.Add( new UICurve( Data.Morph, "Morph" ) );
            Attributes.Add( new UICurve( Data.FresnelCurve, "Fresnel Curve" ) );
            Attributes.Add( new UICurve3Axis( Data.FresnelRotation, "Fresnel Rotation" ) );
            Attributes.Add( new UICurveColor( Data.ColorBegin, "Color Begin" ) );
            Attributes.Add( new UICurveColor( Data.ColorEnd, "Color End" ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            ModelSelect.Draw( id );
            DrawList( Attributes, id );
        }

        public override void Cleanup() {
            ModelSelect.DeleteSelect();
        }
    }
}
