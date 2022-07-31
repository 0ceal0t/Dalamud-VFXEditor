using System.Collections.Generic;
using VFXEditor.AVFX.Views;

namespace VFXEditor.AVFX.VFX {
    public class UITimelineItemSequencer : ImGuiSequencer<UITimelineItem> {
        public UITimeline Timeline;

        public UITimelineItemSequencer( List<UITimelineItem> items, UITimeline timeline ) : base( items ) {
            Timeline = timeline;
        }

        public override int GetEnd( UITimelineItem item ) => item.EndTime.Value;

        public override int GetStart( UITimelineItem item ) => item.StartTime.Value;

        public override UITimelineItem OnNew() {
            var newItem = Timeline.Timeline.AddItem();
            newItem.BinderIdx.SetValue( -1 );
            newItem.EffectorIdx.SetValue( -1 );
            newItem.EmitterIdx.SetValue( -1 );
            newItem.EndTime.SetValue( 1 );
            newItem.Platform.SetValue( 0 );

            return new UITimelineItem( newItem, Timeline );
        }

        public override void OnDelete( UITimelineItem item ) {
            item.BinderSelect.DeleteSelect();
            item.EmitterSelect.DeleteSelect();
            item.EffectorSelect.DeleteSelect();
            Timeline.Timeline.RemoveItem( item.Item );
        }

        public override void SetEnd( UITimelineItem item, int end ) {
            item.EndTime.Value = end;
            item.EndTime.Literal.SetValue( end );
        }

        public override void SetStart( UITimelineItem item, int start ) {
            item.StartTime.Value = start;
            item.StartTime.Literal.SetValue( start );
        }
    }
}
