using AVFXLib.Models;
using System;
using System.Linq;
using AVFXLib.AVFX;

namespace VFXEditor.Avfx.Vfx {
    public class UIEffectorView : UIDropdownView<UIEffector> {
        public UIEffectorView( AvfxFile main, AVFXBase avfx ) : base( main, avfx, "##EFFCT", "Select an Effector", defaultPath: "effector_default.vfxedit" ) {
            Group = main.Effectors;
            Group.Items = AVFX.Effectors.Select( item => new UIEffector( Main, item ) ).ToList();
        }

        public override void OnDelete( UIEffector item ) {
            AVFX.RemoveEffector( item.Effector );
        }

        public override byte[] OnExport( UIEffector item ) {
            return item.Effector.ToAVFX().ToBytes();
        }

        public override UIEffector OnImport( AVFXNode node, bool has_dependencies = false ) {
            var item = new AVFXEffector();
            item.Read( node );
            AVFX.AddEffector( item );
            return new UIEffector( Main, item, has_dependencies );
        }
    }
}
