using System;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public class UITimelineClipSplitView : UIItemSplitView<UITimelineClip> {
        public UITimeline Timeline;

        public UITimelineClipSplitView( List<UITimelineClip> items, UITimeline timeline ) : base( items, true, true ) {
            Timeline = timeline;
        }

        public override UITimelineClip OnNew() {
            return new UITimelineClip( Timeline.Timeline.AddClip(), Timeline );
        }

        public override void OnDelete( UITimelineClip item ) {
            Timeline.Timeline.RemoveClip( item.Clip );
        }
    }
}
