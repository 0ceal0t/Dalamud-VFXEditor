using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataDecal : UIData {
        public AVFXParticleDataDecal Data;

        public UIParticleDataDecal(AVFXParticleDataDecal data)
        {
            Data = data;
            //=======================
            Attributes.Add(new UIFloat("Scaling Scale", Data.ScalingScale));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
