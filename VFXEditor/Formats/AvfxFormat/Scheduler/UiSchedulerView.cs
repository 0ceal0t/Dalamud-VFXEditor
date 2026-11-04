using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Formats.AvfxFormat.Views.Nodes;

namespace VfxEditor.Formats.AvfxFormat.Scheduler {
    public class UiScheduleView : AvfxNodeDropdownView<AvfxScheduler> {
        public UiScheduleView( AvfxFile file, NodeGroup<AvfxScheduler> group ) : base( file, group, "Scheduler", false, false, "" ) { }

        public override void OnSelect( AvfxScheduler item ) { }

        // Should never be called
        public override AvfxScheduler Read( BinaryReader reader, int size ) => null;
    }
}