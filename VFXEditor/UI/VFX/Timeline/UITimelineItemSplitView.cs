using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimelineItemSplitView : UISplitView
    {
        public UITimeline Timeline;
        public UITimelineItemSplitView( List<UIBase> items, UITimeline timeline ) : base( items, true )
        {
            Timeline = timeline;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Item" + id ) )
            {
                Timeline.Timeline.addItem();
                Timeline.Init();
            }
        }
    }
}
