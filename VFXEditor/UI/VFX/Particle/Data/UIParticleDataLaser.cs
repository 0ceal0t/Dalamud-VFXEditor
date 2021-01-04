using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataLaser : UIBase
    {
        public AVFXParticleDataLaser Data;
        //==========================

        public UIParticleDataLaser(AVFXParticleDataLaser data)
        {
            Data = data;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=======================
            Attributes.Add(new UICurve(Data.Width, "Width"));
            Attributes.Add(new UICurve(Data.Length, "Length"));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawAttrs( id );
        }
    }
}
