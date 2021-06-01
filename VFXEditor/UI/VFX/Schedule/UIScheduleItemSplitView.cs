using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UIScheduleItemSplitView : UIItemSplitView<UISchedulerItem> {
        public UIScheduler Sched;

        public UIScheduleItemSplitView( List<UISchedulerItem> items, UIScheduler sched ) : base( items, true, true ) {
            Sched = sched;
        }

        public override UISchedulerItem OnNew() {
            return new UISchedulerItem( Sched.Scheduler.AddItem(), "Item", Sched );
        }

        public override void OnDelete( UISchedulerItem item ) {
            item.TimelineSelect.DeleteSelect();
            Sched.Scheduler.RemoveItem( item.Item );
        }
    }
}
