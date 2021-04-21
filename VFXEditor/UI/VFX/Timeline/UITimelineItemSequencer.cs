using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.UI.Views;

namespace VFXEditor.UI.VFX {
    public class UITimelineItemSequencer : UISequencerView<UITimelineItem> {

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
            var newItem = Timeline.Timeline.addItem();
            newItem.EndTime.GiveValue( (int) Max );
            return new UITimelineItem( newItem, Timeline );
        }

        public override void OnDelete( UITimelineItem item ) {
            item.BinderSelect.DeleteSelect();
            item.EmitterSelect.DeleteSelect();
            item.EffectorSelect.DeleteSelect();
            Timeline.Timeline.removeItem( item.Item );
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
