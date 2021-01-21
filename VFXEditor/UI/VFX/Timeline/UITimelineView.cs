using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UITimelineView : UIDropdownView<UITimeline>
    {
        public UITimelineView(AVFXBase avfx) : base(avfx, "##TIME", "Select a Timeline" )
        {
            List<UITimeline> items = AVFX.Timelines.Select( item => new UITimeline( item, this ) ).ToList();
            UINode._Timelines = new UINodeGroup<UITimeline>( items );
            Group = UINode._Timelines;
        }

        public override UITimeline OnNew()
        {
            return new UITimeline(AVFX.addTimeline(), this);
        }
        public override void OnDelete( UITimeline item )
        {
            AVFX.removeTimeline( item.Timeline );
        }
        public override byte[] OnExport( UITimeline item )
        {
            return item.Timeline.toAVFX().toBytes();
        }
        public override UITimeline OnImport( AVFXNode node ) {
            AVFXTimeline item = new AVFXTimeline();
            item.read( node );
            AVFX.addTimeline( item );
            return new UITimeline( item, this );
        }
    }
}
