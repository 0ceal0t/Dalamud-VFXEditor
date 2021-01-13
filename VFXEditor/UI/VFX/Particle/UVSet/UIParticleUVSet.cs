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

        public override void DrawSelect( int idx, string parentId, ref UIItem selected )
        {
            if( ImGui.Selectable( GetText(idx) + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/UV";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Particle.Particle.removeUvSet( UVSet );
                Particle.UVSplit.OnDelete( this );
                return;
            }
            DrawAttrs( id );
        }

        public override string GetText( int idx ) {
            return "UV " + idx;
        }
    }
}
