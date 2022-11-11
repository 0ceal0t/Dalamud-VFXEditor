using System.Collections.Generic;
using VfxEditor.AVFXLib.Scheduler;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiScheduleItemSplitView : UiItemSplitView<UiSchedulerItem> {
        public UiScheduler Scheduler;

        public UiScheduleItemSplitView( List<UiSchedulerItem> items, UiScheduler scheduler ) : base( items ) {
            Scheduler = scheduler;
        }

        public override void RemoveFromAvfx( UiSchedulerItem item ) {
            item.TimelineSelect.Disable();
            Scheduler.Scheduler.Items.Remove( item.Item );
        }

        public override void AddToAvfx( UiSchedulerItem item, int idx ) {
            item.TimelineSelect.Enable();
            Scheduler.Scheduler.Items.Insert( idx, item.Item );
        }

        public override UiSchedulerItem CreateNewAvfx() => new( new AVFXSchedulerSubItem(), Scheduler, "Item" );
    }
}
