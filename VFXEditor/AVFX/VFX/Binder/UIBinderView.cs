using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Binder;

namespace VFXEditor.AVFX.VFX {
    public class UIBinderView : UIDropdownView<UIBinder> {
        public UIBinderView( AVFXFile main, AVFXMain avfx ) : base( main, avfx, "##BIND", "Select a Binder", defaultPath: "binder_default.vfxedit" ) {
            Group = main.Binders;
            Group.Items = AVFX.Binders.Select( item => new UIBinder( Main, item ) ).ToList();
        }

        public override void OnDelete( UIBinder item ) {
            AVFX.RemoveBinder( item.Binder );
        }

        public override void OnExport( BinaryWriter writer, UIBinder item ) => item.Write( writer );

        public override UIBinder OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXBinder();
            item.Read( reader, size );
            AVFX.AddBinder( item );
            return new UIBinder( Main, item );
        }
    }
}
