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
    public class UIScheduleView : UIDropdownView<UIScheduler> {
        public UIScheduleView( UIMain main, AVFXBase avfx ) : base( main, avfx, "##SCHED", "Select a Scheduler", allowNew:false, allowDelete:false ) {
            Group = main.Schedulers;
            Group.Items = AVFX.Schedulers.Select( item => new UIScheduler( main, item ) ).ToList();
        }

        public override void OnDelete( UIScheduler item ) {}
        public override byte[] OnExport( UIScheduler item ) {
            return new byte[0];
        }
        public override UIScheduler OnImport( AVFXNode node, bool imported = false ) { return null; }
    }
}
