using SharpDX.Direct3D11;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public abstract class MdlMeshDrawable {
        protected Buffer Data; // starts as null

        public abstract uint GetIndexCount();

        public abstract void RefreshBuffer( Device device );

        public Buffer GetBuffer( Device device ) {
            if( Data == null ) RefreshBuffer( device );
            return Data;
        }
    }
}
