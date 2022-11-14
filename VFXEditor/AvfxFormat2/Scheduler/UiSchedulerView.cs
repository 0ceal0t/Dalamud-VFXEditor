using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiScheduleView : UiNodeDropdownView<AvfxScheduler> {
        public UiScheduleView( AvfxFile file, UiNodeGroup<AvfxScheduler> group ) : base( file, group, "Scheduler", false, false, "" ) { }

        public override void OnSelect( AvfxScheduler item ) { }

        // Should never be called
        public override AvfxScheduler Read( BinaryReader reader, int size, bool hasDependencies ) => null;
    }
}