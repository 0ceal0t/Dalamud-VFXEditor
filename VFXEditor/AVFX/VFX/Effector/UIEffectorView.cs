using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Effector;

namespace VFXEditor.AVFX.VFX {
    public class UIEffectorView : UINodeDropdownView<UIEffector> {
        public UIEffectorView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<UIEffector> group ) : base( vfxFile, avfx, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void OnDelete( UIEffector item ) => AVFX.RemoveEffector( item.Effector );

        public override void OnExport( BinaryWriter writer, UIEffector item ) => item.Write( writer );

        public override UIEffector OnImport( BinaryReader reader, int size, bool hasDependencies = false ) {
            var item = new AVFXEffector();
            item.Read( reader, size );
            AVFX.AddEffector( item );
            return new UIEffector( item, hasDependencies );
        }

        public override void OnSelect( UIEffector item ) { }
    }
}
