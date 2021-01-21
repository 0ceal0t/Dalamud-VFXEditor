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
            List<UIScheduler> items = AVFX.Schedulers.Select( item => new UIScheduler( item, this ) ).ToList();
            UINode._Schedulers = new UINodeGroup<UIScheduler>( items );
            Group = UINode._Schedulers;
        }

        public override UIScheduler OnNew() { return null; }
        public override void OnDelete( UIScheduler item ) {}
        public override byte[] OnExport( UIScheduler item ) {
            return new byte[0];
        }
        public override UIScheduler OnImport( AVFXNode node ) { return null; }
    }
}
