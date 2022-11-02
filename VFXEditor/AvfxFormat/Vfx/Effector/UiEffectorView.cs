using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorView : UiNodeDropdownView<UiEffector> {
        public UiEffectorView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiEffector> group ) : base( vfxFile, avfx, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void OnDelete( UiEffector item ) => AVFX.RemoveEffector( item.Effector );

        public override void OnExport( BinaryWriter writer, UiEffector item ) => item.Write( writer );

        public override UiEffector OnImport( BinaryReader reader, int size, bool hasDependencies = false ) {
            var item = new AVFXEffector();
            item.Read( reader, size );
            AVFX.AddEffector( item );
            return new UiEffector( item, hasDependencies );
        }

        public override void OnSelect( UiEffector item ) { }
    }
}
