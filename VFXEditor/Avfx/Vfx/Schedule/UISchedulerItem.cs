using AVFXLib.Models;
using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public class UISchedulerItem : UIWorkspaceItem {
        public AVFXScheduleSubItem Item;
        public UIScheduler Sched;
        public string Name;
        public UINodeSelect<UITimeline> TimelineSelect;
        private readonly List<UIBase> Parameters;

        public UISchedulerItem( AVFXScheduleSubItem item, UIScheduler sched, string name ) {
            Item = item;
            Sched = sched;
            Name = name;

            TimelineSelect = new UINodeSelect<UITimeline>( Sched, "Target Timeline", Sched.Main.Timelines, Item.TimelineIdx );
            Parameters = new List<UIBase> {
                new UICheckbox( "Enabled", Item.Enabled ),
                new UIInt( "Start Time", Item.StartTime )
            };
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.Draw( id );
            DrawList( Parameters, id );
        }

        public override string GetDefaultText() => $"{Idx}: Timeline {Item.TimelineIdx.Value}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Sched.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
