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
        public AVFXBase AVFX;

        public UIBinderView(AVFXBase avfx) : base( "##BIND", "Select a Binder" )
        {
            AVFX = avfx;
            Init();
        }

        public override void Init()
        {
            base.Init();
            foreach( var binder in AVFX.Binders ) {
                var item = new UIBinder( binder, this );
                Items.Add( item );
            }
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
