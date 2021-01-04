using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleUVSet : UIBase
    {
        public AVFXParticleUVSet UVSet;
        public UIParticle Particle;
        //=======================

        public UIParticleUVSet(AVFXParticleUVSet uvSet, UIParticle particle)
        {
            UVSet = uvSet;
            Particle = particle;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //=================
            Attributes.Add(new UICombo<TextureCalculateUV>("Rotation Direction Base", UVSet.CalculateUVType));
            Attributes.Add(new UICurve2Axis(UVSet.Scale, "Scale"));
            Attributes.Add(new UICurve2Axis(UVSet.Scroll, "Scroll"));
            Attributes.Add(new UICurve(UVSet.Rot, "Rotation"));
            Attributes.Add(new UICurve(UVSet.RotRandom, "Rotation Random"));
        }

        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                return;
            }
            if( ImGui.Selectable( "UV " + Idx + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/UV" + Idx;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Particle.Particle.removeUvSet( Idx );
                Particle.Init();
            }
            DrawAttrs( id );
        }
    }
}
