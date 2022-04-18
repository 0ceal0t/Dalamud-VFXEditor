using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib.Scheduler;

namespace VFXEditor.AVFX.VFX {
    public class UIScheduler : UINode {
        public AVFXScheduler Scheduler;
        public AVFXFile Main;
        public List<UISchedulerItem> Items;
        public List<UISchedulerItem> Triggers;
        public UIScheduleItemSplitView ItemSplit;
        public UIItemSplitView<UISchedulerItem> TriggerSplit;

        public UIScheduler( AVFXFile main, AVFXScheduler scheduler ) : base( UINodeGroup.SchedColor, false ) {
            Scheduler = scheduler;
            Main = main;
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

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Scheduler";

            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Items" + id ) ) {
                    ItemSplit.Draw( id + "/Item" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Triggers" + id ) ) {
                    TriggerSplit.Draw( id + "/Trigger" );
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
