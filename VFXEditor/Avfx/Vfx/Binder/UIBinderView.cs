using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.Avfx.Vfx {
    public class UIBinderView : UIDropdownView<UIBinder> {
        public UIBinderView( AvfxFile main, AVFXBase avfx ) : base( main, avfx, "##BIND", "Select a Binder", defaultPath: "binder_default.vfxedit" ) {
            Group = main.Binders;
            Group.Items = AVFX.Binders.Select( item => new UIBinder( Main, item ) ).ToList();
        }

        public override void OnDelete( UIBinder item ) {
            AVFX.RemoveBinder( item.Binder );
        }

        public override byte[] OnExport( UIBinder item ) {
            return item.Binder.ToAVFX().ToBytes();
        }

        public override UIBinder OnImport( AVFXNode node, bool has_dependencies = false ) {
            var item = new AVFXBinder();
            item.Read( node );
            AVFX.AddBinder( item );
            return new UIBinder( Main, item );
        }
    }
}
