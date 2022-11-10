using System.IO;
using System.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterView : UiNodeDropdownView<UiEmitter> {
        public UiEmitterView( AvfxFile vfxFile, AVFXMain avfx, UiNodeGroup<UiEmitter> group ) : base( vfxFile, avfx, group, "Emitter", true, true, "default_emitter.vfxedit" ) { }

        public override void RemoveFromAvfx( UiEmitter item ) => Avfx.Emitters.Remove( item.Emitter );

        public override void AddToAvfx( UiEmitter item, int idx ) => Avfx.Emitters.Insert( idx, item.Emitter );

        public override void OnExport( BinaryWriter writer, UiEmitter item ) => item.Write( writer );

        public override UiEmitter OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXEmitter();
            item.Read( reader, size );
            Avfx.Emitters.Add( item );
            return new UiEmitter( item, VfxFile.NodeGroupSet, has_dependencies );
        }

        public override void OnSelect( UiEmitter item ) { }
    }
}
