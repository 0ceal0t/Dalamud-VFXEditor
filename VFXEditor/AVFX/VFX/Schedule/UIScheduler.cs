using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib.Scheduler;

namespace VFXEditor.AVFX.VFX {
    public class UIScheduler : UINode {
        public readonly AVFXScheduler Scheduler;
        public readonly UINodeGroupSet NodeGroups;
        public readonly List<UISchedulerItem> Items;
        public readonly List<UISchedulerItem> Triggers;
        public readonly UIScheduleItemSplitView ItemSplit;
        public readonly UIItemSplitView<UISchedulerItem> TriggerSplit;

        public UIScheduler( AVFXScheduler scheduler, UINodeGroupSet nodeGroups ) : base( UINodeGroup.SchedColor, false ) {
            Scheduler = scheduler;
            NodeGroups = nodeGroups;
            Items = new List<UISchedulerItem>();
            Triggers = new List<UISchedulerItem>();

            foreach( var Item in Scheduler.Items ) {
                Items.Add( new UISchedulerItem( Item, this, "Item" ) );
            }

            foreach( var Trigger in Scheduler.Triggers ) {
                Triggers.Add( new UISchedulerItem( Trigger, this, "Trigger" ) );
            }

            ItemSplit = new UIScheduleItemSplitView( Items, this );
            TriggerSplit = new UIItemSplitView<UISchedulerItem>( Triggers );
            HasDependencies = false; // if imported, all set now
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Scheduler";

            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.DrawInline( id + "/Item" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Triggers" + id ) ) {
                    TriggerSplit.DrawInline( id + "/Trigger" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
            Triggers.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
            Triggers.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
        }

        public override string GetDefaultText() => $"Scheduler {Idx}";

        public override string GetWorkspaceId() => $"Sched{Idx}";

        public override void Write( BinaryWriter writer ) => Scheduler.Write( writer );
    }
}
