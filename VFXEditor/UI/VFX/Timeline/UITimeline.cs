using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITimeline : UIBase
    {
        public AVFXTimeline Timeline;
        public UITimelineView View;
        public int Idx;
        //=====================
        public List<UITimelineItem> Items;
        //=====================
        public List<UITimelineClip> Clips;

        public UITimeline(AVFXTimeline timeline, UITimelineView view)
        {
            Timeline = timeline;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Items = new List<UITimelineItem>();
            Clips = new List<UITimelineClip>();
            //========================
            Attributes.Add(new UIInt("Loop Start", Timeline.LoopStart));
            Attributes.Add(new UIInt("Loop End", Timeline.LoopEnd));
            Attributes.Add(new UIInt("Binder Index", Timeline.BinderIdx));
            //========================
            foreach (var item in Timeline.Items)
            {
                Items.Add(new UITimelineItem(item, this));
            }
            //==========================
            foreach (var clip in Timeline.Clips)
            {
                Clips.Add(new UITimelineClip(clip, this));
            }
        }

        public override void Draw(string parentId)
        {
            string id = parentId + "/Timeline" + Idx;
            if (ImGui.CollapsingHeader("Timeline " + Idx + id))
            {
                if (UIUtils.RemoveButton("Delete" + id))
                {
                    View.AVFX.removeTimeline(Idx);
                    View.Init();
                }
                if (ImGui.TreeNode("Parameters" + id))
                {
                    DrawAttrs(id);
                    ImGui.TreePop();
                }
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
                        Timeline.addItem();
                        Init();
                    }
                    ImGui.TreePop();
                }
                //=====================
                if (ImGui.TreeNode("Clips (" + Clips.Count() + ")" + id))
                {
                    int cIdx = 0;
                    foreach (var clip in Clips)
                    {
                        clip.Idx = cIdx;
                        clip.Draw(id);
                        cIdx++;
                    }
                    if (ImGui.Button("+ Clip" + id))
                    {
                        Timeline.addClip();
                        Init();
                    }
                    ImGui.TreePop();
                }
            }
        }
    }
}
