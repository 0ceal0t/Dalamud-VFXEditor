using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduleItemSplitView : UISplitView
    {
        public UIScheduler Sched;
        public UIScheduleItemSplitView( List<UIBase> items, UIScheduler sched ) : base( items, true )
        {
            Sched = sched;
        }

        public override void DrawNewButton( string id )
        {
            if( ImGui.Button( "+ Item" + id ) )
            {
                Sched.Scheduler.addItem();
                Sched.Init();
            }
        }
    }
}
