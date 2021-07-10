using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduler : UINode {
        public AVFXSchedule Scheduler;
        public UIMain Main;
        // =================
        public List<UISchedulerItem> Items;
        public List<UISchedulerItem> Triggers;
        // ================
        public UIScheduleItemSplitView ItemSplit;
        public UIItemSplitView<UISchedulerItem> TriggerSplit;

        public UIScheduler(UIMain main, AVFXSchedule scheduler) : base( UINodeGroup.SchedColor, false ) {
            Scheduler = scheduler;
            Main = main;
            // ===================
            Items = new List<UISchedulerItem>();
            Triggers = new List<UISchedulerItem>();
            // =====================
            foreach( var Item in Scheduler.Items ) {
                Items.Add( new UISchedulerItem( Item, this, "Item") );
            }
            // =====================
            foreach( var Trigger in Scheduler.Triggers ) {
                Triggers.Add( new UISchedulerItem( Trigger, this, "Trigger" ) );
            }
            // ======================
            ItemSplit = new UIScheduleItemSplitView( Items, this );
            TriggerSplit = new UIItemSplitView<UISchedulerItem>( Triggers );
            HasDependencies = false; // if imported, all set now
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Scheduler";
            //=====================
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
        public override string GetDefaultText() {
            return "Scheduler " + Idx;
        }

        public override string GetWorkspaceId() {
            return $"Sched{Idx}";
        }

        public override void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
            Triggers.ForEach( item => item.PopulateWorkspaceMeta( RenameDict ) );
        }

        public override void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) {
            Items.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
            Triggers.ForEach( item => item.ReadWorkspaceMetaChildren( RenameDict ) );
        }

        public override byte[] ToBytes() {
            return Scheduler.ToAVFX().ToBytes();
        }
    }
}
