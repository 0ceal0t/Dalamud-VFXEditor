using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UITimelineItemSplitView : UIItemSplitView<UITimelineItem> {
        public UITimeline Timeline;

        public UITimelineItemSplitView( List<UITimelineItem> items, UITimeline timeline ) : base( items, true, true ) {
            Timeline = timeline;
        }

        public override UITimelineItem OnNew() {
            return new UITimelineItem( Timeline.Timeline.addItem(), Timeline );
        }

        public override void OnDelete( UITimelineItem item ) {
            item.BinderSelect.DeleteSelect();
            item.EmitterSelect.DeleteSelect();
            item.EffectorSelect.DeleteSelect();
            Timeline.Timeline.removeItem( item.Item );
        }
    }
}
