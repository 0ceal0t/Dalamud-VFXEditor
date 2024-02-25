using SharpDX.Direct3D11;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Utils;
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

        protected void PopulateIndexData( MdlFileData data, BinaryReader reader, int lod ) {
            var padding = data.IndexBufferPositions[lod].Dequeue() - ( ( IndexCount * 2 ) + _IndexOffset );
            reader.BaseStream.Position = data.IndexBufferOffsets[lod] + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 + padding ) );
        }

        public abstract void Draw();
    }
}
