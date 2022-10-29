using System.Collections.Generic;

namespace VfxEditor.AVFX.VFX {
    public class UIScheduleItemSplitView : UIItemSplitView<UISchedulerItem> {
        public UIScheduler Sched;

        public UIScheduleItemSplitView( List<UISchedulerItem> items, UIScheduler sched ) : base( items, true, true ) {
            Sched = sched;
        }

        public override UISchedulerItem OnNew() {
            return new UISchedulerItem( Sched.Scheduler.Add(), Sched, "Item" );
        }

        public override void OnDelete( UISchedulerItem item ) {
            item.TimelineSelect.DeleteSelect();
            Sched.Scheduler.Remove( item.Item );
        }
    }
}
