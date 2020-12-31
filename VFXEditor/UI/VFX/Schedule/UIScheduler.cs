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
        public int Idx;
        // =================
        public List<UISchedulerItem> Items;
        public List<UISchedulerItem> Triggers;

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
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Scheduler" + Idx;
            if (ImGui.CollapsingHeader("Scheduler " + Idx + id))
            {
                //=====================
                if (ImGui.TreeNode("Items (" + Items.Count() + ")" + id))
                {
                    int iIdx = 0;
                    foreach (var item in Items)
                    {
                        item.Idx = iIdx;
                        item.Draw(id);
                        iIdx++;
                    }
                    if (ImGui.Button("+ Item" + id))
                    {
                        Scheduler.addItem();
                        Init();
                    }
                    ImGui.TreePop();
                }
                //=====================
                if (ImGui.TreeNode("Triggers (" + Triggers.Count() + ")" + id))
                {
                    int tIdx = 0;
                    foreach (var trigger in Triggers)
                    {
                        trigger.Idx = tIdx;
                        trigger.Draw(id);
                        tIdx++;
                    }
                    ImGui.TreePop();
                }
            }
        }
    }
}
