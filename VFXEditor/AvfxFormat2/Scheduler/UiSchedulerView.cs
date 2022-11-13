using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiScheduleView : UiNodeDropdownView<AvfxScheduler> {
        public UiScheduleView( AvfxFile file, UiNodeGroup<AvfxScheduler> group ) : base( file, group, "Scheduler", false, false, "" ) { }

        public override void OnSelect( AvfxScheduler item ) { }
    }
}