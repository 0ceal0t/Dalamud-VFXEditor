using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UIBinderView : UIDropdownView<UIBinder>
    {
        public UIBinderView( UIMain main, AVFXBase avfx ) : base( main, avfx, "##BIND", "Select a Binder", defaultPath:"binder_default.vfxedit" )
        {
            Group = UINode._Binders;
            Group.Items = AVFX.Binders.Select( item => new UIBinder( item, this ) ).ToList();
        }
        public override void OnDelete( UIBinder item ) {
            AVFX.removeBinder( item.Binder );
        }
        public override byte[] OnExport( UIBinder item ) {
            return item.Binder.toAVFX().toBytes();
        }
        public override UIBinder OnImport( AVFXNode node, bool has_dependencies = false ) {
            AVFXBinder item = new AVFXBinder();
            item.read( node );
            AVFX.addBinder( item );
            return new UIBinder( item, this );
        }
    }
}
