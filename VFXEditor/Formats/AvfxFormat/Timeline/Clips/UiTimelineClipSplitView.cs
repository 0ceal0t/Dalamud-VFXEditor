using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiTimelineClipSplitView : AvfxItemSplitView<AvfxTimelineClip> {
        public readonly AvfxTimeline Timeline;

        public UiTimelineClipSplitView( List<AvfxTimelineClip> items, AvfxTimeline timeline ) : base( "Clips", items ) {
            Timeline = timeline;
        }

        public override AvfxTimelineClip CreateNewAvfx() => new( Timeline );
    }
}
