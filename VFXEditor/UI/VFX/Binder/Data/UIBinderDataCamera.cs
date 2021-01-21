using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIBinderDataCamera : UIBase
    {
        public AVFXBinderDataCamera Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //=======================

        public UIBinderDataCamera(AVFXBinderDataCamera data)
        {
            Data = data;
            //==================
            Attributes.Add(new UICurve(data.Distance, "Distance"));
            Attributes.Add( new UICurve( data.DistanceRandom, "Distance Random" ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
