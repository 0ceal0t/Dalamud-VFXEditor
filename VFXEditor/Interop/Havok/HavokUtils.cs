using System.Numerics;

namespace VfxEditor.Interop.Havok {
    public static class HavokUtils {
        public static Matrix4x4 CleanMatrix( Matrix4x4 matrix ) {
            var newMatrix = new Matrix4x4 {
                M11 = CleanFloat( matrix.M11 ),
                M12 = CleanFloat( matrix.M12 ),
                M13 = CleanFloat( matrix.M13 ),
                M14 = CleanFloat( matrix.M14 ),

                M21 = CleanFloat( matrix.M21 ),
                M22 = CleanFloat( matrix.M22 ),
                M23 = CleanFloat( matrix.M23 ),
                M24 = CleanFloat( matrix.M24 ),

                M31 = CleanFloat( matrix.M31 ),
                M32 = CleanFloat( matrix.M32 ),
                M33 = CleanFloat( matrix.M33 ),
                M34 = CleanFloat( matrix.M34 ),

                M41 = CleanFloat( matrix.M41 ),
                M42 = CleanFloat( matrix.M42 ),
                M43 = CleanFloat( matrix.M43 ),
                M44 = CleanFloat( matrix.M44 )
            };

            return newMatrix;
        }

        private static float CleanFloat( float m ) {
            if( m > 0.999 && m < 1 ) return 1;
            else if( m > -0.001 && m < 0 ) return 0;
            else if( m < 0.001 && m > 0 ) return 0;
            return m;
        }
    }
}
