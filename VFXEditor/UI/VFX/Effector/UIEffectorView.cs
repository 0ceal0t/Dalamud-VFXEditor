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
        public UIEffectorView(AVFXBase avfx) : base(avfx, "##EFFCT", "Select an Effector" )
        {
            List<UIEffector> items = AVFX.Effectors.Select( item => new UIEffector( item, this ) ).ToList();
            UINode._Effectors = new UINodeGroup<UIEffector>( items );
            Group = UINode._Effectors;
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
