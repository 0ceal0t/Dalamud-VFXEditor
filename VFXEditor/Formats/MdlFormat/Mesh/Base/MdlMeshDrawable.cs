using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.DirectX;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlMeshDrawable {
        public readonly int RenderId = Renderer.NewId;
        protected Buffer Data; // starts as null

        protected uint _IndexOffset;

        protected uint StartOffset => _IndexOffset / 2;

        protected uint IndexCount;

        protected byte[] RawIndexData = [];

        public int RawIndexDataSize => RawIndexData.Length;

        public uint GetIndexCount() => IndexCount;

        public abstract void RefreshBuffer( Device device );

        public Buffer GetBuffer( Device device ) {
            if( GetIndexCount() == 0 ) return null;
            if( Data == null ) RefreshBuffer( device );
            return Data;
        }

        protected void Populate( BinaryReader reader, uint indexBufferPos ) {
            reader.BaseStream.Position = indexBufferPos + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 ) );
        }
    }
}
