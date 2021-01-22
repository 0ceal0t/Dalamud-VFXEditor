using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleDataDecalRing : UIData {
        public AVFXParticleDataDecalRing Data;
        public List<UIBase> Attributes = new List<UIBase>();
        //==========================

        public UIParticleDataDecalRing(AVFXParticleDataDecalRing data)
        {
            Data = data;
            //=======================
            Attributes.Add( new UICurve( Data.Width, "Width" ) );
            Attributes.Add( new UIFloat( "Scaling Scale", Data.ScalingScale ) );
            Attributes.Add( new UIFloat( "Ring Fan", Data.RingFan ) );
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Data";
            DrawList( Attributes, id );
        }
    }
}
