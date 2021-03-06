using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VFXEditor.UI.VFX {
    public class UITimelineClipSplitView : UIItemSplitView<UITimelineClip> {
        public UITimeline Timeline;

        public UITimelineClipSplitView( List<UITimelineClip> items, UITimeline timeline ) : base( items, true, true ) {
            Timeline = timeline;
        }

        public override UITimelineClip OnNew() {
            return new UITimelineClip( Timeline.Timeline.addClip(), Timeline );
        }

        public override void OnDelete( UITimelineClip item ) {
            Timeline.Timeline.removeClip( item.Clip );
        }
    }
}
