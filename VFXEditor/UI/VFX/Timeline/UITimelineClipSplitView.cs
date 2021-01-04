using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimelineClipSplitView : UISplitView
    {
        public UITimeline Timeline;
        public UITimelineClipSplitView( List<UIBase> items, UITimeline timeline ) : base( items, true )
        {
            Timeline = timeline;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Clip" + id ) )
            {
                Timeline.Timeline.addClip();
                Timeline.Init();
            }
        }
    }
}
