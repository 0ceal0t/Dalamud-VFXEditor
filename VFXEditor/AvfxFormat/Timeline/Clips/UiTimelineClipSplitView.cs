using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiTimelineClipSplitView : UiItemSplitView<AvfxTimelineClip> {
        public readonly AvfxTimeline Timeline;

        public UiTimelineClipSplitView( List<AvfxTimelineClip> items, AvfxTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override void Disable( AvfxTimelineClip item ) { }

        public override void Enable( AvfxTimelineClip item ) { }

        public override AvfxTimelineClip CreateNewAvfx() => new AvfxTimelineClip( Timeline );
    }
}
