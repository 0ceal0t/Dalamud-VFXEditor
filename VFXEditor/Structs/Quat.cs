
using System.Runtime.InteropServices;

namespace VfxEditor.Structs {
    [StructLayout( LayoutKind.Sequential )]
    public struct Quat {
        public float X;
        public float Z;
        public float Y;
        public float W;

        public static implicit operator System.Numerics.Vector4( Quat pos ) => new( pos.X, pos.Y, pos.Z, pos.W );

        public static implicit operator SharpDX.Vector4( Quat pos ) => new( pos.X, pos.Z, pos.Y, pos.W );
    }
}
