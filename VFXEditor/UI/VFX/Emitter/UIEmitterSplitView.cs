using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterSplitView : UISplitView
    {
        public UIEmitter Emitter;
        public bool IsParticle;

        public UIEmitterSplitView( List<UIBase> items, UIEmitter emitter, bool isParticle ) : base( items, true )
        {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override void DrawNewButton( string parentId )
        {
            string Type = IsParticle ? "Particle" : "Emitter";
            if( ImGui.Button( "+ " + Type + parentId ) )
            {
                if( IsParticle )
                {
                    Emitter.Emitter.addParticle();
                    Emitter.Init();
                }
                else
                {
                    Emitter.Emitter.addEmitter();
                    Emitter.Init();
                }
            }
        }
    }
}
