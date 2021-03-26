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
        public UIScheduleView View;
        // =================
        public List<UISchedulerItem> Items;
        public List<UISchedulerItem> Triggers;
        // ================
        public UIScheduleItemSplitView ItemSplit;
        public UIItemSplitView<UISchedulerItem> TriggerSplit;

        public UIScheduler(AVFXSchedule scheduler, UIScheduleView view)
        {
            Scheduler = scheduler;
            View = view;
            _Color = TextureColor;

            // ===================
            Items = new List<UISchedulerItem>();
            Triggers = new List<UISchedulerItem>();
            // =====================
            foreach( var Item in Scheduler.Items ) {
                Items.Add( new UISchedulerItem( Item, "Item", this ) );
            }
            // =====================
            foreach( var Trigger in Scheduler.Triggers ) {
                Triggers.Add( new UISchedulerItem( Trigger, "Trigger", this ) );
            }
            // ======================
            ItemSplit = new UIScheduleItemSplitView( Items, this );
            TriggerSplit = new UIItemSplitView<UISchedulerItem>( Triggers );
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
        public override string GetText() {
            return "Scheduler " + Idx;
        }

        public override byte[] toBytes() {
            return Scheduler.toAVFX().toBytes();
        }
    }
}
