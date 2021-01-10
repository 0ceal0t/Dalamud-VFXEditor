using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduler : UIBase
    {
        public AVFXSchedule Scheduler;
        public UIScheduleView View;
        // =================
        public List<UISchedulerItem> Items;
        public List<UISchedulerItem> Triggers;
        // ================
        public UIScheduleItemSplitView ItemSplit;
        public UISplitView<UISchedulerItem> TriggerSplit;

        public UIScheduler(AVFXSchedule scheduler, UIScheduleView view)
        {
            Scheduler = scheduler;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            // ===================
            Items = new List<UISchedulerItem>();
            Triggers = new List<UISchedulerItem>();
            // =====================
            foreach (var Item in Scheduler.Items)
            {
                Items.Add(new UISchedulerItem(Item, "Item", this));
            }
            // =====================
            foreach (var Trigger in Scheduler.Triggers)
            {
                Triggers.Add(new UISchedulerItem(Trigger, "Trigger", this));
            }
            // ======================
            ItemSplit = new UIScheduleItemSplitView( Items, this );
            TriggerSplit = new UISplitView<UISchedulerItem>( Triggers );
        }

        public string GetDescText()
        {
            return "Scheduler " + Idx;
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Scheduler" + Idx;
            //=====================
            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) )
            {
                if( ImGui.BeginTabItem( "Items" + id ) )
                {
                    DrawItems( id + "/Item" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Triggers" + id ) )
                {
                    DrawTriggers( id + "/Trigger" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        private void DrawItems(string id )
        {
            ItemSplit.Draw( id );
        }
        private void DrawTriggers(string id )
        {
            TriggerSplit.Draw( id );
        }
    }
}
