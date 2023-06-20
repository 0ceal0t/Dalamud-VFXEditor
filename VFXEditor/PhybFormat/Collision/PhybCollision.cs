using System.IO;

namespace VfxEditor.PhybFormat.Collision {
    public class PhybCollision {
        public readonly PhybFile File;

        //public readonly List<PhybCapsule> Capsules = new();


        public PhybCollision( PhybFile file, BinaryReader reader ) {
            File = file;

            var numCapsules = reader.ReadByte();
            var numEllipsoids = reader.ReadByte();
            var numNormalPlanes = reader.ReadByte();
            var numThreePointPlanes = reader.ReadByte();
            var numSpheres = reader.ReadByte();
            reader.ReadBytes( 3 ); // padding, 0xCC, 0xCC, 0xCC
        }

        public void Write( BinaryWriter writer ) {

        }
    }
}
