using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataLaser : UIData {
        public AVFXParticleDataLaser Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIParticleDataLaser(AVFXParticleDataLaser data)
        {
            Data = data;
            //=======================
            Attributes.Add( new UICurve( Data.Width, "Width" ) );
            Attributes.Add( new UICurve( Data.Length, "Length" ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
