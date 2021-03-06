using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleUVSet : UIItem
    {
        public AVFXParticleUVSet UVSet;
        public UIParticle Particle;

        public UIParticleUVSet(AVFXParticleUVSet uvSet, UIParticle particle)
        {
            UVSet = uvSet;
            Particle = particle;
            //=================
            Attributes.Add( new UICombo<TextureCalculateUV>( "Rotation Direction Base", UVSet.CalculateUVType ) );
            Attributes.Add( new UICurve2Axis( UVSet.Scale, "Scale" ) );
            Attributes.Add( new UICurve2Axis( UVSet.Scroll, "Scroll" ) );
            Attributes.Add( new UICurve( UVSet.Rot, "Rotation" ) );
            Attributes.Add( new UICurve( UVSet.RotRandom, "Rotation Random" ) );
        }

        public override void DrawBody( string parentId )
        {
            string id = parentId + "/UV";
            DrawAttrs( id );
        }

        public override string GetText() {
            return "UV " + Idx;
        }
    }
}
