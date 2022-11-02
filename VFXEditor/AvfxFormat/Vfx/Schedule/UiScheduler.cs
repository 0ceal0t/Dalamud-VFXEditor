using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib.Scheduler;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiScheduler : UiNode {
        public readonly AVFXScheduler Scheduler;
        public readonly UiNodeGroupSet NodeGroups;
        public readonly List<UiSchedulerItem> Items;
        public readonly List<UiSchedulerItem> Triggers;
        public readonly UiScheduleItemSplitView ItemSplit;
        public readonly UiItemSplitView<UiSchedulerItem> TriggerSplit;

        public UiScheduler( AVFXScheduler scheduler, UiNodeGroupSet nodeGroups ) : base( UiNodeGroup.SchedColor, false ) {
            Scheduler = scheduler;
            NodeGroups = nodeGroups;
            Items = new List<UiSchedulerItem>();
            Triggers = new List<UiSchedulerItem>();

            foreach( var Item in Scheduler.Items ) {
                Items.Add( new UiSchedulerItem( Item, this, "Item" ) );
            }

            foreach( var Trigger in Scheduler.Triggers ) {
                Triggers.Add( new UiSchedulerItem( Trigger, this, "Trigger" ) );
            }

            ItemSplit = new UiScheduleItemSplitView( Items, this );
            TriggerSplit = new UiItemSplitView<UiSchedulerItem>( Triggers );
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
