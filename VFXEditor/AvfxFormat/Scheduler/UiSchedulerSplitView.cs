using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiSchedulerSplitView : UiItemSplitView<AvfxSchedulerItem> {
        public readonly AvfxScheduler Scheduler;
        public readonly bool IsItem;

        public UiSchedulerSplitView( List<AvfxSchedulerItem> items, AvfxScheduler scheduler, bool isItem ) : base( items ) {
            if( !isItem ) {
                AllowDelete = false;
                AllowNew = false;
            }
            Scheduler = scheduler;
            IsItem = isItem;
        }

        public override void Disable( AvfxSchedulerItem item ) => item.TimelineSelect.Disable();

        public override void Enable( AvfxSchedulerItem item ) => item.TimelineSelect.Enable();

        public override AvfxSchedulerItem CreateNewAvfx() => new( Scheduler, IsItem ? "Item" : "Trgr", true );
    }
}
