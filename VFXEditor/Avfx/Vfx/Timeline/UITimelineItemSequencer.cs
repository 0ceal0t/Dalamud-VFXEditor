using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Avfx.Views;

namespace VFXEditor.Avfx.Vfx {
    public class UITimelineItemSequencer : ImGuiSequencer<UITimelineItem> {

        public UITimeline Timeline;

        public UITimelineItemSequencer( List<UITimelineItem> items, UITimeline timeline ) : base(items) {
            Timeline = timeline;
        }

        public override int GetEnd( UITimelineItem item ) {
            return item.EndTime.Value;
        }

        public override int GetStart( UITimelineItem item ) {
            return item.StartTime.Value;
        }

        public override UITimelineItem OnNew() {
            var newItem = Timeline.Timeline.AddItem();
            newItem.EndTime.GiveValue( 1 );
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
            item.EndTime.Literal.GiveValue( end );
        }

        public override void SetStart( UITimelineItem item, int start ) {
            item.StartTime.Value = start;
            item.StartTime.Literal.GiveValue( start );
        }
    }
}