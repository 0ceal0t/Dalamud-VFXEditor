using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffectorDataCameraQuake : UIData {
        public AVFXEffectorDataCameraQuake Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIEffectorDataCameraQuake(AVFXEffectorDataCameraQuake data)
        {
            Data = data;
            //=======================
            Attributes.Add(new UICurve(Data.Attenuation, "Attenuation"));
            Attributes.Add(new UICurve(Data.RadiusOut, "Radius Out"));
            Attributes.Add(new UICurve(Data.RadiusIn, "Radius In"));
            Attributes.Add(new UICurve3Axis(Data.Rotation, "Rotation"));
            Attributes.Add(new UICurve3Axis(Data.Position, "Position"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
