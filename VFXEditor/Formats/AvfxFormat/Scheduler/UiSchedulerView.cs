using System.IO;

namespace VfxEditor.AvfxFormat {
    public class UiScheduleView : AvfxNodeDropdownView<AvfxScheduler> {
        public UiScheduleView( AvfxFile file, NodeGroup<AvfxScheduler> group ) : base( file, group, "Scheduler", false, false, "" ) { }

        public override void OnSelect( AvfxScheduler item ) { }

        // Should never be called
        public override AvfxScheduler Read( BinaryReader reader, int size ) => null;
    }
}