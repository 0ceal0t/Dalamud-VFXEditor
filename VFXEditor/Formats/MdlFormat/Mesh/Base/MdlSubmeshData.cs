using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlSubmeshData<T> : MdlMeshDrawable, IUiItem where T : MdlMeshData {
        private T Parent;

        public MdlSubmeshData( T parent ) {
            Parent = parent;
        }

        public void Populate( T parent, BinaryReader reader, uint indexBufferPos ) {
            Parent = parent;
            Populate( reader, indexBufferPos );
        }

        public override void RefreshBuffer( Device device ) {
            if( Parent == null ) return;

            Data?.Dispose();
            var data = Parent.GetData( ( int )IndexCount, RawIndexData );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        protected void DrawPreview() {
            if( Parent == null ) return;

            if( Plugin.DirectXManager.MeshPreview.CurrentMesh != this ) {
                Plugin.DirectXManager.MeshPreview.LoadMesh( Parent.File, this );
            }
            Plugin.DirectXManager.MeshPreview.DrawInline();
        }

        public abstract void Draw();
    }
}
