using AVFXLib.AVFX;
using AVFXLib.Models;
using System;
using System.Linq;

namespace VFXEditor.Avfx.Vfx {
    public class UIScheduleView : UIDropdownView<UIScheduler> {
        public UIScheduleView( AvfxFile main, AVFXBase avfx ) : base( main, avfx, "##SCHED", "Select a Scheduler", allowNew: false, allowDelete: false ) {
            Group = main.Schedulers;
            Group.Items = AVFX.Schedulers.Select( item => new UIScheduler( main, item ) ).ToList();
        }

        public override void OnDelete( UIScheduler item ) { }

        public override byte[] OnExport( UIScheduler item ) => Array.Empty<byte>();

        public override UIScheduler OnImport( AVFXNode node, bool has_dependencies = false ) => null;
    }
}
