using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderView : UiNodeDropdownView<UiBinder> {
        public UiBinderView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiBinder> group ) : base( vfxFile, avfx, group, "Binder", true, true, "default_binder.vfxedit" ) { }

        public override void OnDelete( UiBinder item ) => AVFX.RemoveBinder( item.Binder );

        public override void OnExport( BinaryWriter writer, UiBinder item ) => item.Write( writer );

        public override UiBinder OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXBinder();
            item.Read( reader, size );
            AVFX.AddBinder( item );
            return new UiBinder( item );
        }

        public override void OnSelect( UiBinder item ) { }
    }
}
