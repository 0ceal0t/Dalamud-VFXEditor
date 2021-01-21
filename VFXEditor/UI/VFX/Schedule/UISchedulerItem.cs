using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VFXEditor.UI.VFX
{
    public class UISchedulerItem : UIItem
    {
        public AVFXScheduleSubItem Item;
        public UIScheduler Sched;
        public string Name;
        // ====================

        public UISchedulerItem(AVFXScheduleSubItem item, string name, UIScheduler sched)
        {
            Item = item;
            Sched = sched;
            Name = name;
            // ============================
            Attributes.Add( new UICheckbox( "Enabled", Item.Enabled ) );
            Attributes.Add( new UIInt( "Start Time", Item.StartTime ) );
            Attributes.Add( new UIInt( "Timeline Index", Item.TimelineIdx ) );
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/" + Name;
            DrawAttrs( id );
        }

        public override string GetText() {
            return Idx + ": Timeline " + Item.TimelineIdx.Value;
        }
    }
}
