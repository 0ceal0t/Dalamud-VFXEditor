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
        public UIBinderView(AVFXBase avfx) : base(avfx, "##BIND", "Select a Binder" )
        {
            List<UIBinder> items = AVFX.Binders.Select( item => new UIBinder( item, this ) ).ToList();
            UINode._Binders = new UINodeGroup<UIBinder>( items );
            Group = UINode._Binders;
        }
        public override UIBinder OnNew() {
            return new UIBinder(AVFX.addBinder(), this);
        }
        public override void OnDelete( UIBinder item ) {
            AVFX.removeBinder( item.Binder );
        }
        public override byte[] OnExport( UIBinder item ) {
            return item.Binder.toAVFX().toBytes();
        }
        public override UIBinder OnImport( AVFXNode node ) {
            AVFXBinder item = new AVFXBinder();
            item.read( node );
            AVFX.addBinder( item );
            return new UIBinder( item, this );
        }
    }
}
