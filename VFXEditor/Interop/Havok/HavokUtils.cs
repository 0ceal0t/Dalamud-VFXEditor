using SharpDX;

namespace VfxEditor.Interop.Havok {
    public static class HavokUtils {
        public static Matrix CleanMatrix( Matrix matrix ) {
            var newMatrix = new Matrix();
            for( var i = 0; i < 16; i++ ) {
                var m = matrix[i];
                if( m > 0.999 && m < 1 ) {
                    newMatrix[i] = 1;
                }
                else if( m > -0.001 && m < 0 ) {
                    newMatrix[i] = 0;
                }
                else if( m < 0.001 && m > 0 ) {
                    newMatrix[i] = 0;
                }
                else {
                    newMatrix[i] = m;
                }
            }
            return newMatrix;
        }
    }
}
