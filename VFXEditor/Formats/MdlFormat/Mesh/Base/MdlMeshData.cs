using SharpDX.Direct3D11;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlMeshData : MdlMeshDrawable, IUiItem {
        public readonly MdlFile File;

        public MdlMeshData( MdlFile file ) {
            File = file;
        }

        public abstract Vector4[] GetData( int indexCount, byte[] rawIndexData );

        public override void RefreshBuffer( Device device ) {
            Data?.Dispose();
            var data = GetData( ( int )IndexCount, RawIndexData );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        protected void DrawPreview() {
            if( Plugin.DirectXManager.MeshPreview.CurrentRenderId != RenderId ) {
                Plugin.DirectXManager.MeshPreview.LoadMesh( this );
            }
            Plugin.DirectXManager.MeshPreview.DrawInline();
        }

        public abstract void Draw();
    }
}
