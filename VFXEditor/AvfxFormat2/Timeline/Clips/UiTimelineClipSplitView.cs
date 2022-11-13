using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
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
