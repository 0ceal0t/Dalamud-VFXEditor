using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Model;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelView : UiNodeSplitView<UiModel> {
        public UiModelView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiModel> group ) : base( vfxFile, avfx, group, "Model", true, true, "default_model.vfxedit2" ) { }

        public override void OnSelect( UiModel item ) => item.OnSelect();

        public override void OnDelete( UiModel item ) => Avfx.RemoveModel( item.Model );

        public override UiModel OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var mdl = new AVFXModel();
            mdl.Read( reader, size );
            Avfx.AddModel( mdl );
            return new UiModel( mdl );
        }
    }
}
