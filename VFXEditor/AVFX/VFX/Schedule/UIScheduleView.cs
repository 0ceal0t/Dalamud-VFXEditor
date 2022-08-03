using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;

namespace VFXEditor.AVFX.VFX {
    public class UIScheduleView : UINodeDropdownView<UIScheduler> {
        public UIScheduleView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<UIScheduler> group ) : base( vfxFile, avfx, group, "Scheduler", false, false, "" ) { }

        public override void OnDelete( UIScheduler item ) { }

        public override void OnExport( BinaryWriter writer, UIScheduler item ) { }

        public override UIScheduler OnImport( BinaryReader reader, int size, bool has_dependencies = false ) => null;

        public override void OnSelect( UIScheduler item ) { }
    }
}
