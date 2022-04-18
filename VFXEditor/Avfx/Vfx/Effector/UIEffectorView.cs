using System;
using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Effector;

namespace VFXEditor.AVFX.VFX {
    public class UIEffectorView : UIDropdownView<UIEffector> {
        public UIEffectorView( AVFXFile main, AVFXMain avfx ) : base( main, avfx, "##EFFCT", "Select an Effector", defaultPath: "effector_default.vfxedit" ) {
            Group = main.Effectors;
            Group.Items = AVFX.Effectors.Select( item => new UIEffector( Main, item ) ).ToList();
        }

        public override void OnDelete( UIEffector item ) {
            AVFX.RemoveEffector( item.Effector );
        }

        public override void OnExport( BinaryWriter writer, UIEffector item ) => item.Write( writer );

        public override UIEffector OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXEffector();
            item.Read( reader, size );
            AVFX.AddEffector( item );
            return new UIEffector( Main, item, has_dependencies );
        }
    }
}
