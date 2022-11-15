using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat2 {
    public class UiSchedulerSplitView : UiItemSplitView<AvfxSchedulerSubItem> {
        public readonly AvfxScheduler Scheduler;
        public readonly bool IsItem;

        public UiSchedulerSplitView( List<AvfxSchedulerSubItem> items, AvfxScheduler scheduler, bool isItem ) : base( items ) {
            Scheduler = scheduler;
            IsItem = isItem;
        }

        public override void Disable( AvfxSchedulerSubItem item ) => item.TimelineSelect.Disable();

        public override void Enable( AvfxSchedulerSubItem item ) => item.TimelineSelect.Enable();

        public override AvfxSchedulerSubItem CreateNewAvfx() => new( Scheduler, IsItem ? "Item" : "Trgr", true );
    }
}
