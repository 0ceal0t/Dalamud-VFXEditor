using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VFXEditor.UI.VFX
{
    public class UISchedulerItem : UIWorkspaceItem {
        public AVFXScheduleSubItem Item;
        public UIScheduler Sched;
        public string Name;
        // ====================

        public UINodeSelect<UITimeline> TimelineSelect;

        public UISchedulerItem(AVFXScheduleSubItem item, string name, UIScheduler sched)
        {
            Item = item;
            Sched = sched;
            Name = name;

            TimelineSelect = new UINodeSelect<UITimeline>( Sched, "Target Timeline", UINodeGroup.Timelines, Item.TimelineIdx );
            // ============================
            Attributes.Add( new UICheckbox( "Enabled", Item.Enabled ) );
            Attributes.Add( new UIInt( "Start Time", Item.StartTime ) );
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.Draw(id);
            DrawAttrs( id );
        }

        public override string GetDefaultText() {
            return Idx + ": Timeline " + Item.TimelineIdx.Value;
        }
    }
}
