using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiScheduleView : UiNodeDropdownView<UiScheduler> {
        public UiScheduleView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiScheduler> group ) : base( vfxFile, avfx, group, "Scheduler", false, false, "" ) { }

        public override void RemoveFromAvfx( UiScheduler item ) { }

        public override void AddToAvfx( UiScheduler item, int idx ) { }

        public override void OnExport( BinaryWriter writer, UiScheduler item ) { }

        public override UiScheduler AddToAvfx( BinaryReader reader, int size, bool hasDepdencies ) => null;

        public override void OnSelect( UiScheduler item ) { }
    }
}
