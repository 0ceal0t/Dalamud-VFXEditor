using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineClipSplitView : UiItemSplitView<UiTimelineClip> {
        public UiTimeline Timeline;

        public UiTimelineClipSplitView( List<UiTimelineClip> items, UiTimeline timeline ) : base( items, true, true ) {
            Timeline = timeline;
        }

        public override UiTimelineClip OnNew() {
            return new UiTimelineClip( Timeline.Timeline.AddClip(), Timeline );
        }

        public override void OnDelete( UiTimelineClip item ) {
            Timeline.Timeline.RemoveClip( item.Clip );
        }
    }
}
