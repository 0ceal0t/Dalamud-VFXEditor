using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiScheduleItemSplitView : UiItemSplitView<UiSchedulerItem> {
        public UiScheduler Sched;

        public UiScheduleItemSplitView( List<UiSchedulerItem> items, UiScheduler sched ) : base( items, true, true ) {
            Sched = sched;
        }

        public override UiSchedulerItem OnNew() {
            return new UiSchedulerItem( Sched.Scheduler.Add(), Sched, "Item" );
        }

        public override void OnDelete( UiSchedulerItem item ) {
            item.TimelineSelect.Disable();
            Sched.Scheduler.Remove( item.Item );
        }
    }
}
