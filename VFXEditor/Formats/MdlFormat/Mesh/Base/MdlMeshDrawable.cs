using SharpDX.Direct3D11;
using System;
using VfxEditor.DirectX;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlMeshDrawable {
        public readonly MdlFile File;
        public readonly int RenderId = RenderInstance.NewId;

        protected Buffer Data; // starts as null

        protected uint _IndexOffset;

        public uint IndexBufferOffset => _IndexOffset;

        protected uint IndexCount;

        protected byte[] RawIndexData = [];

        public int RawIndexDataSize => RawIndexData.Length;

        public MdlMeshDrawable( MdlFile file ) {
            File = file; 
        }

        public uint GetIndexCount() => IndexCount;

        public abstract void RefreshBuffer( Device device );

        public Buffer GetBuffer( Device device ) {
            if( GetIndexCount() == 0 ) return null;
            if( Data == null ) RefreshBuffer( device );
            return Data;
        }

        protected virtual void DrawPreview() {
            Plugin.DirectXManager.MeshRenderer.DrawTexture( RenderId, File.MeshInstance, UpdateRender, null );
        }

        protected void UpdateRender() {
            Plugin.DirectXManager.MeshRenderer.SetMesh( RenderId, File.MeshInstance, this );
        }
    }
}
