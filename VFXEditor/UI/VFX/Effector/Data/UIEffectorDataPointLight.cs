using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorDataPointLight : UIData {
        public AVFXEffectorDataPointLight Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIEffectorDataPointLight(AVFXEffectorDataPointLight data)
        {
            Data = data;
            //=======================
            Attributes.Add(new UICurveColor(Data.Color, "Color"));
            Attributes.Add(new UICurve(Data.DistanceScale, "Distance Scale"));
            Attributes.Add(new UICurve3Axis(Data.Rotation, "Rotation"));
            Attributes.Add(new UICurve3Axis(Data.Position, "Position"));
            Attributes.Add(new UICombo<PointLightAttenuation>("Point Light Attenuation", Data.PointLightAttenuationType));
            Attributes.Add(new UICheckbox("Enable Shadow", Data.EnableShadow));
            Attributes.Add(new UICheckbox("Enable Char Shadow", Data.EnableCharShadow));
            Attributes.Add(new UICheckbox("Enable Map Shadow", Data.EnableMapShadow));
            Attributes.Add(new UICheckbox("Enabled Move Shadow", Data.EnableMoveShadow));
            Attributes.Add(new UIFloat("Create Distance Near", Data.ShadowCreateDistanceNear));
            Attributes.Add(new UIFloat("Create Distance Far", Data.ShadowCreateDistanceFar));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
