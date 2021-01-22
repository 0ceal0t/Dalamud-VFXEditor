using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIBinderDataPoint : UIData {
        public AVFXBinderDataPoint Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //=======================

        public UIBinderDataPoint(AVFXBinderDataPoint data)
        {
            Data = data;
            //==================
            Attributes.Add(new UICurve(data.SpringStrength, "Spring Strength"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
