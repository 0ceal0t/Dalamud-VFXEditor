using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Model;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelView : UiNodeSplitView<UiModel> {
        public UiModelView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiModel> group ) : base( vfxFile, avfx, group, "Model", true, true, "default_model.vfxedit2" ) { }

        public override void OnSelect( UiModel item ) => item.OnSelect();

        public override void RemoveFromAvfx( UiModel item ) => Avfx.Models.Remove( item.Model );

        public override void AddToAvfx( UiModel item, int idx ) => Avfx.Models.Insert( idx, item.Model );

        public override UiModel AddToAvfx( BinaryReader reader, int size, bool hasDependencies ) {
            var mdl = new AVFXModel();
            mdl.Read( reader, size );
            Avfx.Models.Add( mdl );
            return new UiModel( mdl );
        }
    }
}
