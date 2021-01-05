using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIUVSetSplitView : UISplitView
    {
        public UIParticle Particle;
        public UIUVSetSplitView(List<UIBase> items, UIParticle particle) : base(items, true )
        {
            Particle = particle;
        }

        public override void DrawNewButton( string id )
        {
            if( Particle.UVSets.Count < 4 )
            {
                if( ImGui.Button( "+ UVSet" + id ) )
                {
                    var p = Particle.Particle.addUvSet();
                    if(p != null )
                    {
                        Particle.UVSets.Add( new UIParticleUVSet( p, Particle ) );
                    }
                }
            }
        }
    }
}
