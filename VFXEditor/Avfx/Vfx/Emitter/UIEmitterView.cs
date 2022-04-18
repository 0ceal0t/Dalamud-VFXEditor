using System;
using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Emitter;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterView : UIDropdownView<UIEmitter> {
        public UIEmitterView( AVFXFile main, AVFXMain avfx ) : base( main, avfx, "##EMIT", "Select an Emitter", defaultPath: "emitter_default.vfxedit" ) {
            Group = main.Emitters;
            Group.Items = AVFX.Emitters.Select( item => new UIEmitter( Main, item ) ).ToList();
        }

        public override void OnDelete( UIEmitter item ) {
            AVFX.RemoveEmitter( item.Emitter );
        }

        public override void OnExport( BinaryWriter writer, UIEmitter item ) => item.Write( writer );

        public override UIEmitter OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXEmitter();
            item.Read( reader, size );
            AVFX.AddEmitter( item );
            return new UIEmitter( Main, item, has_dependencies );
        }
    }
}
