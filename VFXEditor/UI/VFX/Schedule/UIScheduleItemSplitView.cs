using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduleItemSplitView : UISplitView<UISchedulerItem>
    {
        public UIScheduler Sched;
        public UIScheduleItemSplitView( List<UISchedulerItem> items, UIScheduler sched ) : base( items, true )
        {
            Sched = sched;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Item" + id ) )
            {
                Sched.Items.Add( new UISchedulerItem( Sched.Scheduler.addItem(), "Item", Sched ) );
            }
        }
    }
}
