using System.Numerics;

namespace VfxEditor.Interop.Havok {
    public static class HavokUtils {
        public static Matrix4x4 CleanMatrix( Matrix4x4 matrix ) {
            var newMatrix = new Matrix4x4();
            newMatrix.M11 = CleanFloat( matrix.M11 );
            newMatrix.M12 = CleanFloat( matrix.M12 );
            newMatrix.M13 = CleanFloat( matrix.M13 );
            newMatrix.M14 = CleanFloat( matrix.M14 );

            newMatrix.M21 = CleanFloat( matrix.M21 );
            newMatrix.M22 = CleanFloat( matrix.M22 );
            newMatrix.M23 = CleanFloat( matrix.M23 );
            newMatrix.M24 = CleanFloat( matrix.M24 );

            newMatrix.M31 = CleanFloat( matrix.M31 );
            newMatrix.M32 = CleanFloat( matrix.M32 );
            newMatrix.M33 = CleanFloat( matrix.M33 );
            newMatrix.M34 = CleanFloat( matrix.M34 );

            newMatrix.M41 = CleanFloat( matrix.M41 );
            newMatrix.M42 = CleanFloat( matrix.M42 );
            newMatrix.M43 = CleanFloat( matrix.M43 );
            newMatrix.M44 = CleanFloat( matrix.M44 );

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
