using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduleView : UIBase
    {
        public AVFXBase AVFX;
        List<UIScheduler> Schedulers;
        public int Selected = -1;
        public string[] Options;

        public UIScheduleView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Schedulers = new List<UIScheduler>();
            Options = new string[AVFX.Schedulers.Count];
            int idx = 0;
            foreach( var sched in AVFX.Schedulers )
            {
                var item = new UIScheduler( sched, this );
                item.Idx = idx;
                Options[idx] = item.GetDescText();
                Schedulers.Add( item );
                idx++;
            }
        }
        public void RefreshDesc( int idx )
        {
            Options[idx] = Schedulers[idx].GetDescText();
        }
        public override void Draw(string parentId = "")
        {
            string id = "##SCHED";
            bool validSelect = UIUtils.ViewSelect( id, "Select a Scheduler", ref Selected, Options );
            ImGui.Separator();
            // ====================
            if( validSelect )
            {
                Schedulers[Selected].Draw( id );
            }
        }
    }
}
