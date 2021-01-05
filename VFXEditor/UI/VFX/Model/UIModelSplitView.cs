using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModelSplitView : UISplitView
    {
        public UIModelView ModelView;
        public UIModelSplitView( List<UIBase> items, UIModelView modelView ) : base( items, true )
        {
            ModelView = modelView;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Model" + id ) )
            {
                ModelView.Models.Add( new UIModel( ModelView.AVFX.addModel(), ModelView ) );
            }
        }
    }
}
