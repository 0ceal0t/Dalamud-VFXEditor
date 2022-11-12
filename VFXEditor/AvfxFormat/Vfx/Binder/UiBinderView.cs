using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderView : UiNodeDropdownView<UiBinder> {
        public UiBinderView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiBinder> group ) : base( vfxFile, avfx, group, "Binder", true, true, "default_binder.vfxedit" ) { }

        public override void RemoveFromAvfx( UiBinder item ) => Avfx.Binders.Remove( item.Binder );

        public override void AddToAvfx( UiBinder item, int idx ) => Avfx.Binders.Insert( idx, item.Binder );

        public override void OnExport( BinaryWriter writer, UiBinder item ) => item.Write( writer );

        public override UiBinder AddToAvfx( BinaryReader reader, int size, bool hasDepdencies ) {
            var item = new AVFXBinder();
            item.Read( reader, size );
            Avfx.Binders.Add( item );
            return new UiBinder( item );
        }

        public override void OnSelect( UiBinder item ) { }
    }
}
