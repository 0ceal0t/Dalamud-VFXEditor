using AVFXLib.AVFX;
using AVFXLib.Models;
using System.Linq;

namespace VFXEditor.Avfx.Vfx {
    public class UIModelView : UINodeSplitView<UIModel> {
        public UIModelView(AvfxFile main, AVFXBase avfx) : base(main, avfx, "##MDL") {
            Group = main.Models;
            Group.Items = AVFX.Models.Select( item => new UIModel( Main, item ) ).ToList();
        }

        public override void OnSelect( UIModel item ) {
            Plugin.DirectXManager.ModelView.LoadModel( item.Model );
        }

        public override void OnDelete( UIModel item ) {
            AVFX.RemoveModel( item.Model );
        }

        public override UIModel OnNew() {
            return new UIModel( Main, AVFX.AddModel() );
        }

        public override UIModel OnImport( AVFXNode node, bool has_dependencies = false ) {
            var mdl = new AVFXModel();
            mdl.Read( node );
            AVFX.AddModel( mdl );
            return new UIModel( Main, mdl );
        }
    }
}
