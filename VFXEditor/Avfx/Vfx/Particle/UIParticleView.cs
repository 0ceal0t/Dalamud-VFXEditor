using System;
using System.IO;
using System.Linq;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;

namespace VFXEditor.Avfx.Vfx {
    public class UIParticleView : UIDropdownView<UIParticle> {
        public UIParticleView( AvfxFile main, AVFXMain avfx ) : base( main, avfx, "##PTCL", "Select a Particle", defaultPath: "particle_default.vfxedit" ) {
            Group = Main.Particles;
            Group.Items = AVFX.Particles.Select( item => new UIParticle( Main, item ) ).ToList();
        }

        public override void OnDelete( UIParticle item ) {
            AVFX.RemoveParticle( item.Particle );
        }

        public override void OnExport( BinaryWriter writer, UIParticle item ) => item.Write( writer );

        public override UIParticle OnImport( BinaryReader reader, int size, bool has_dependencies = false ) {
            var item = new AVFXParticle();
            item.Read( reader, size );
            AVFX.AddParticle( item );
            return new UIParticle( Main, item, has_dependencies );
        }
    }
}
