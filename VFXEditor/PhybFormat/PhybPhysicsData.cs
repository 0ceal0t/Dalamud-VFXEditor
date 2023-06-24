using System.IO;
using System.Linq;

namespace VfxEditor.PhybFormat {
    public abstract class PhybPhysicsData : PhybData {
        public PhybPhysicsData( PhybFile file ) : base( file ) { }

        public PhybPhysicsData( PhybFile file, BinaryReader reader ) : base( file, reader ) { }

        public override void Draw() {
            if( Parsed.Aggregate( false, ( x, y ) => y.Draw( CommandManager.Phyb ) || x ) ) File.PhysicsUpdated = true;
        }
    }
}
