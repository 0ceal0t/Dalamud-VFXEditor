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
            DrawAttrs( id );
        }
    }
}
