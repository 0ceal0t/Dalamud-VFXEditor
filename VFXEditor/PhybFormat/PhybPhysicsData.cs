using System.IO;

namespace VfxEditor.PhybFormat {
    public abstract class PhybPhysicsData : PhybData {
        public PhybPhysicsData( PhybFile file ) : base( file ) { }

        public PhybPhysicsData( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        public override void Draw() => Parsed.ForEach( x => x.Draw( CommandManager.Phyb ) );
    }
}
