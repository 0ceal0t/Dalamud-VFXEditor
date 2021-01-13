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
    public class UIEffectorView : UIDropdownView<UIEffector>
    {
        public AVFXBase AVFX;

        public UIEffectorView(AVFXBase avfx) : base( "##EFFCT", "Select an Effector" )
        {
            AVFX = avfx;
            //===================
            foreach( var effector in AVFX.Effectors ) {
                var item = new UIEffector( effector, this );
                Items.Add( item );
            }
        }

        public override UIEffector OnNew() {
            return new UIEffector(AVFX.addEffector(), this);
        }
        public override void OnDelete( UIEffector item ) {
            AVFX.removeEffector( item.Effector );
        }
        public override byte[] OnExport( UIEffector item ) {
            return item.Effector.toAVFX().toBytes();
        }
        public override UIEffector OnImport( AVFXNode node ) {
            AVFXEffector item = new AVFXEffector();
            item.read( node );
            AVFX.addEffector( item );
            return new UIEffector( item, this );
        }
    }
}
