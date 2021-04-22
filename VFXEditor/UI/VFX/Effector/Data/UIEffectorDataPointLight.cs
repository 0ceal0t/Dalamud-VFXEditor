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
        public UIParameters Parameters;

        public UIEffectorDataPointLight(AVFXEffectorDataPointLight data)
        {
            Data = data;
            //=======================
            Tabs.Add( Parameters = new UIParameters( "Parameters" ) );
            Parameters.Add( new UICombo<PointLightAttenuation>( "Point Light Attenuation", Data.PointLightAttenuationType ) );
            Parameters.Add( new UICheckbox( "Enable Shadow", Data.EnableShadow ) );
            Parameters.Add( new UICheckbox( "Enable Char Shadow", Data.EnableCharShadow ) );
            Parameters.Add( new UICheckbox( "Enable Map Shadow", Data.EnableMapShadow ) );
            Parameters.Add( new UICheckbox( "Enabled Move Shadow", Data.EnableMoveShadow ) );
            Parameters.Add( new UIFloat( "Create Distance Near", Data.ShadowCreateDistanceNear ) );
            Parameters.Add( new UIFloat( "Create Distance Far", Data.ShadowCreateDistanceFar ) );

            Tabs.Add(new UICurveColor(Data.Color, "Color"));
            Tabs.Add(new UICurve(Data.DistanceScale, "Distance Scale"));
            Tabs.Add(new UICurve3Axis(Data.Rotation, "Rotation"));
            Tabs.Add(new UICurve3Axis(Data.Position, "Position"));
        }
    }
}
