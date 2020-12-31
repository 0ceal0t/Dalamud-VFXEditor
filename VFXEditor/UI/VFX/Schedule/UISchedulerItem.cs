using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VFXEditor.UI.VFX
{
    public class UISchedulerItem : UIBase
    {
        public AVFXScheduleSubItem Item;
        public UIScheduler Sched;
        public int Idx;
        public string Name;
        // ====================

        public UISchedulerItem(AVFXScheduleSubItem item, string name, UIScheduler sched)
        {
            Item = item;
            Sched = sched;
            Name = name;
            Init();
        }
        public override void Init()
        {
            base.Init();
            // ============================
            Attributes.Add(new UICheckbox("Enabled", Item.Enabled));
            Attributes.Add(new UIInt("Start Time", Item.StartTime));
            Attributes.Add(new UIInt("Timeline Index", Item.TimelineIdx));
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/" + Name + Idx;
            if (ImGui.TreeNode(Name + " " + Idx + id))
            {
                if(Name == "Item")
                {
                    if (UIUtils.RemoveButton("Delete" + id))
                    {
                        Sched.Scheduler.removeItem(Idx);
                        Sched.Init();
                    }
                }
                DrawAttrs(id);
                ImGui.TreePop();
            }
        }
    }
}
