using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;

namespace VFXEditor.AVFX.VFX {
    public class UIScheduleView : UIDropdownView<UIScheduler> {
        public UIScheduleView( AVFXFile main, AVFXMain avfx ) : base( main, avfx, "##SCHED", "Select a Scheduler", allowNew: false, allowDelete: false ) {
            Group = main.Schedulers;
            Group.Items = AVFX.Schedulers.Select( item => new UIScheduler( main, item ) ).ToList();
        }

        public override void OnDelete( UIScheduler item ) { }

        public override void OnExport( BinaryWriter writer, UIScheduler item ) { }

        public override UIScheduler OnImport( BinaryReader reader, int size, bool has_dependencies = false ) => null;
    }
}
