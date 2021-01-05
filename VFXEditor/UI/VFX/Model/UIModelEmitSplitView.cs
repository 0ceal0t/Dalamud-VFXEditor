using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModelEmitSplitView : UISplitView
    {
        public UIModel Model;
        public UIModelEmitSplitView( List<UIBase> items, UIModel model ) : base( items, true, leftSize:120 )
        {
            Model = model;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Emitter Vertex" + id ) )
            {
                var vnum = Model.Model.addVNum();
                var emit = Model.Model.addEmitVertex();
                Model.EmitterVerts.Add( new UIModelEmitterVertex( vnum, emit, Model ) );
            }
        }
    }
}
