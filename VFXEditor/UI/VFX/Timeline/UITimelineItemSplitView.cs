using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimelineItemSplitView : UISplitView<UITimelineItem>
    {
        public UITimeline Timeline;
        public UITimelineItemSplitView( List<UITimelineItem> items, UITimeline timeline ) : base( items, true )
        {
            Timeline = timeline;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Item" + id ) )
            {
                OnNew( new UITimelineItem( Timeline.Timeline.addItem(), Timeline ) );
            }
        }
    }
}
