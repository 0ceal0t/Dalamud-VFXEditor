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
        public int Selected = -1;
        public string[] Options;

        public UITimelineView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Timelines = new List<UITimeline>();
            Options = new string[AVFX.Timelines.Count];
            int idx = 0;
            foreach( var timeline in AVFX.Timelines )
            {
                var item = new UITimeline( timeline, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Timelines.Add( item );
                idx++;
            }
        }
        public void RefreshDesc( int idx )
        {
            Options[idx] = Timelines[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##TIME";
            bool validSelect = UIUtils.ViewSelect( id, "Select a Timeline", ref Selected, Options );
            ImGui.SameLine();
            if( ImGui.Button( "+ NEW" + id ) )
            {
                AVFX.addTimeline();
                Init();
            }
            if( validSelect )
            {
                ImGui.SameLine();
                if( UIUtils.RemoveButton( "DELETE" + id ) )
                {
                    AVFX.removeTimeline( Selected );
                    Init();
                    validSelect = false;
                }
            }
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Timelines[Selected].Draw( id );
            }
        }
    }
}
