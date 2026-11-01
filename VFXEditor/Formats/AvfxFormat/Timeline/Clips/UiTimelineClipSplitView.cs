using System.Collections.Generic;
using VfxEditor.Formats.AvfxFormat.Views.Items;

namespace VfxEditor.Formats.AvfxFormat.Timeline.Clips {
    public class UiTimelineClipSplitView : AvfxItemSplitView<AvfxTimelineClip> {
        public readonly AvfxTimeline Timeline;

        public UiTimelineClipSplitView( List<AvfxTimelineClip> items, AvfxTimeline timeline ) : base( "Clips", items ) {
            Timeline = timeline;
        }

        public override AvfxTimelineClip CreateNewAvfx() => new( Timeline );
    }
}
