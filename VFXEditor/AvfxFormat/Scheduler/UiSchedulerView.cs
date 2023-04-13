using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiScheduleView : UiNodeDropdownView<AvfxScheduler> {
        public UiScheduleView( AvfxFile file, NodeGroup<AvfxScheduler> group ) : base( file, group, "Scheduler", false, false, "" ) { }

        public override void OnSelect( AvfxScheduler item ) { }

        // Should never be called
        public override AvfxScheduler Read( BinaryReader reader, int size ) => null;
    }
}