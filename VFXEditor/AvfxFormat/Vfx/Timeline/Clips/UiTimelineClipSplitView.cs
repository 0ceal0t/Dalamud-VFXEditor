using System.Collections.Generic;
using VfxEditor.AVFXLib.Timeline;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiTimelineClipSplitView : UiItemSplitView<UiTimelineClip> {
        public UiTimeline Timeline;

        public UiTimelineClipSplitView( List<UiTimelineClip> items, UiTimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override void RemoveFromAvfx( UiTimelineClip item ) {
            Timeline.Timeline.Clips.Remove( item.Clip );
        }

        public override void AddToAvfx( UiTimelineClip item, int idx ) {
            Timeline.Timeline.Clips.Insert( idx, item.Clip );
        }

        public override UiTimelineClip CreateNewAvfx() => new( new AVFXTimelineClip(), Timeline );
    }
}
