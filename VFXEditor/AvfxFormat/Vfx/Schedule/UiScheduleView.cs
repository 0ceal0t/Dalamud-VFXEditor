using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiScheduleView : UiNodeDropdownView<UiScheduler> {
        public UiScheduleView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiScheduler> group ) : base( vfxFile, avfx, group, "Scheduler", false, false, "" ) { }

        public override void OnDelete( UiScheduler item ) { }

        public override void OnExport( BinaryWriter writer, UiScheduler item ) { }

        public override UiScheduler OnImport( BinaryReader reader, int size, bool has_dependencies = false ) => null;

        public override void OnSelect( UiScheduler item ) { }
    }
}
