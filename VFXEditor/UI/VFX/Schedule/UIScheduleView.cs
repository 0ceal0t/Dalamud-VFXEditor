using AVFXLib.Models;
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

        public UIScheduleView(AVFXBase avfx)
        {
            AVFX = avfx;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Schedulers = new List<UIScheduler>();
            foreach (var sched in AVFX.Schedulers)
            {
                Schedulers.Add(new UIScheduler(sched, this));
            }
        }

        public override void Draw(string parentId = "")
        {
            string id = "##SCHED";
            int sIdx = 0;
            foreach (var sched in Schedulers)
            {
                sched.Idx = sIdx;
                sched.Draw(id);
                sIdx++;
            }
        }
    }
}
