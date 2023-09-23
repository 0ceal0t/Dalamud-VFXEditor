using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiSchedulerSplitView : AvfxItemSplitView<AvfxSchedulerItem> {
        public readonly AvfxScheduler Scheduler;
        public readonly bool IsItem;

        public UiSchedulerSplitView( string id, List<AvfxSchedulerItem> items, AvfxScheduler scheduler, bool isItem ) : base( id, items ) {
            if( !isItem ) {
                AllowDelete = false;
                ShowControls = false;
            }
            Scheduler = scheduler;
            IsItem = isItem;
        }

        public override void Disable( AvfxSchedulerItem item ) => item.TimelineSelect.Disable();

        public override void Enable( AvfxSchedulerItem item ) => item.TimelineSelect.Enable();

        public override AvfxSchedulerItem CreateNewAvfx() => new( Scheduler, IsItem ? "Item" : "Trgr", true );
    }
}
