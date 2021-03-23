using AVFXLib.AVFX;
using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIScheduleView : UIDropdownView<UIScheduler>
    {
        public UIScheduleView(AVFXBase avfx) : base(avfx, "##SCHED", "Select a Scheduler", allowNew:false, allowDelete:false )
        {
            Group = UINode._Schedulers;
            Group.Items = AVFX.Schedulers.Select( item => new UIScheduler( item, this ) ).ToList();
        }

        public override void OnDelete( UIScheduler item ) {}
        public override byte[] OnExport( UIScheduler item ) {
            return new byte[0];
        }
        public override UIScheduler OnImport( AVFXNode node ) { return null; }
    }
}
