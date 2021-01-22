using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIBinderDataSpline : UIData {
        public AVFXBinderDataSpline Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //=======================

        public UIBinderDataSpline(AVFXBinderDataSpline data)
        {
            Data = data;
            //==================
            Attributes.Add(new UICurve(data.CarryOverFactor, "Carry Over Factor"));
            Attributes.Add(new UICurve(data.CarryOverFactorRandom, "Carry Over Factor Random"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
