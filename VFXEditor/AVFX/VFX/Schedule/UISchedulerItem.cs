using System.Collections.Generic;
using VfxEditor.AVFXLib.Scheduler;

namespace VfxEditor.AVFX.VFX {
    public class UISchedulerItem : UIWorkspaceItem {
        public AVFXSchedulerSubItem Item;
        public UIScheduler Scheduler;
        public string Name;
        public UINodeSelect<UITimeline> TimelineSelect;
        private readonly List<IUIBase> Parameters;

        public UISchedulerItem( AVFXSchedulerSubItem item, UIScheduler scheduler, string name ) {
            Item = item;
            Scheduler = scheduler;
            Name = name;

            TimelineSelect = new UINodeSelect<UITimeline>( Scheduler, "Target Timeline", Scheduler.NodeGroups.Timelines, Item.TimelineIdx );
            Parameters = new List<IUIBase> {
                new UICheckbox( "Enabled", Item.Enabled ),
                new UIInt( "Start Time", Item.StartTime )
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.DrawInline( id );
            IUIBase.DrawList( Parameters, id );
        }

        public override string GetDefaultText() => $"{Idx}: Timeline {Item.TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
