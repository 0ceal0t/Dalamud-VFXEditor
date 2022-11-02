using System.Collections.Generic;
using VfxEditor.AVFXLib.Scheduler;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiSchedulerItem : UiWorkspaceItem {
        public AVFXSchedulerSubItem Item;
        public UiScheduler Scheduler;
        public string Name;
        public UiNodeSelect<UiTimeline> TimelineSelect;
        private readonly List<IUiBase> Parameters;

        public UiSchedulerItem( AVFXSchedulerSubItem item, UiScheduler scheduler, string name ) {
            Item = item;
            Scheduler = scheduler;
            Name = name;

            TimelineSelect = new UiNodeSelect<UiTimeline>( Scheduler, "Target Timeline", Scheduler.NodeGroups.Timelines, Item.TimelineIdx );
            Parameters = new List<IUiBase> {
                new UiCheckbox( "Enabled", Item.Enabled ),
                new UiInt( "Start Time", Item.StartTime )
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/" + Name;
            DrawRename( id );
            TimelineSelect.DrawInline( id );
            IUiBase.DrawList( Parameters, id );
        }

        public override string GetDefaultText() => $"{Idx}: Timeline {Item.TimelineIdx.GetValue()}";

        public override string GetWorkspaceId() {
            var Type = ( Name == "Item" ) ? "Item" : "Trigger";
            return $"{Scheduler.GetWorkspaceId()}/{Type}{Idx}";
        }
    }
}
