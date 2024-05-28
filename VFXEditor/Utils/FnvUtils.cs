using System.Text;

namespace VfxEditor.Utils {
    public static class FnvUtils {
        public static uint Encode( string value ) {
            var data = Encoding.ASCII.GetBytes( value );
            var res = 0x811c9dc5;
            for( var i = 0; i < data.Length; i++ ) {
                res ^= data[i];
                res *= 0x01000193;
            }
            return res;
        }
    }
}
