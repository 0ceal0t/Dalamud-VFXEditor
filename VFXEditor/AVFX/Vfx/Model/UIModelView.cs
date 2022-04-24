using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Model;

namespace VFXEditor.AVFX.VFX {
    public class UIModelView : UINodeSplitView<UIModel> {
        public UIModelView( AVFXFile main, AVFXMain avfx ) : base( main, avfx, "##MDL" ) {
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

        public override UIModel OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var mdl = new AVFXModel();
            mdl.Read( reader, size );
            AVFX.AddModel( mdl );
            return new UIModel( Main, mdl );
        }
    }
}
