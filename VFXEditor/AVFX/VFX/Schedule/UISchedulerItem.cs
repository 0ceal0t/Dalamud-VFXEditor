using System.Collections.Generic;
using VFXEditor.AVFXLib.Scheduler;

namespace VFXEditor.AVFX.VFX {
    public class UISchedulerItem : UIWorkspaceItem {
        public AVFXSchedulerSubItem Item;
        public UIScheduler Scheduler;
        public string Name;
        public UINodeSelect<UITimeline> TimelineSelect;
        private readonly List<UIBase> Parameters;

        public UISchedulerItem( AVFXSchedulerSubItem item, UIScheduler scheduler, string name ) {
            Item = item;
            Scheduler = scheduler;
            Name = name;

            TimelineSelect = new UINodeSelect<UITimeline>( Scheduler, "Target Timeline", Scheduler.NodeGroups.Timelines, Item.TimelineIdx );
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

        public override string GetDefaultText() => $"{Idx}: Timeline {Item.TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
