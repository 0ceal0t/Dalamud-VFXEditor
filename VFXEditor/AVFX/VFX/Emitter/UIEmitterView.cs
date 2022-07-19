using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterView : UINodeDropdownView<UIEmitter> {
        public UIEmitterView( AVFXFile vfxFile, AVFXMain avfx, UINodeGroup<UIEmitter> group ) : base( vfxFile, avfx, group, "Emitter", true, true, "emitter_default.vfxedit" ) {
        }

        public override void OnDelete( UIEmitter item ) => AVFX.RemoveEmitter( item.Emitter );

        public override void OnExport( BinaryWriter writer, UIEmitter item ) => item.Write( writer );

        public override UIEmitter OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXEmitter();
            item.Read( reader, size );
            AVFX.AddEmitter( item );
            return new UIEmitter( item, VfxFile.NodeGroupSet, has_dependencies );
        }
    }
}
