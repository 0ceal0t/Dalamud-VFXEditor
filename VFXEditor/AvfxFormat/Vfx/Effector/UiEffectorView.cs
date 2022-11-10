using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffectorView : UiNodeDropdownView<UiEffector> {
        public UiEffectorView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiEffector> group ) : base( vfxFile, avfx, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void RemoveFromAvfx( UiEffector item ) => Avfx.Effectors.Remove( item.Effector );

        public override void AddToAvfx( UiEffector item, int idx ) => Avfx.Effectors.Insert( idx, item.Effector );

        public override void OnExport( BinaryWriter writer, UiEffector item ) => item.Write( writer );

        public override UiEffector OnImport( BinaryReader reader, int size, bool hasDependencies = false ) {
            var item = new AVFXEffector();
            item.Read( reader, size );
            Avfx.Effectors.Add( item );
            return new UiEffector( item, hasDependencies );
        }

        public override void OnSelect( UiEffector item ) { }
    }
}
