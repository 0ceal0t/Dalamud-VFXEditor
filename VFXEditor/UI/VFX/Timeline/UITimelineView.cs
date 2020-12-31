using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI.VFX
{
    public class UITimelineView : UIBase
    {
        public AVFXBase AVFX;
        List<UITimeline> Timelines;

        public UITimelineView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Timelines = new List<UITimeline>();
            foreach (var timeline in AVFX.Timelines)
            {
                Timelines.Add(new UITimeline(timeline, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##TIME";
            int tIdx = 0;
            foreach (var timeline in Timelines)
            {
                timeline.Idx = tIdx;
                timeline.Draw(id);
                tIdx++;
            }
            if (ImGui.Button("+ Timeline" + id))
            {
                AVFX.addTimeline();
                Init();
            }
        }
    }
}
