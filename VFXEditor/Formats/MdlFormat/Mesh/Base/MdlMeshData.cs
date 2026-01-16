using SharpDX.Direct3D11;
using System.IO;
using System.Numerics;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh.Base {
    public abstract class MdlMeshData : MdlMeshDrawable, IUiItem {
        public readonly MdlFile File;

        public MdlMeshData( MdlFile file ) : base ( file ) { }

        public abstract Vector4[] GetData( int indexCount, byte[] rawIndexData );

        public override void RefreshBuffer( Device device ) {
            Data?.Dispose();
            var data = GetData( ( int )IndexCount, RawIndexData );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        protected void PopulateIndexData( MdlFileData data, BinaryReader reader, int lod ) {
            // prevents total failure to load MDL if index buffer is malformed for LOD
            // TODO: why/when does this happen when enqueueing?
            if( data.IndexBufferPositions[lod].Count == 0) {
                Dalamud.Error($"IndexBufferPositions Queue empty for LOD #{lod}, aborting populate");
                return;
            }
            var padding = data.IndexBufferPositions[lod].Dequeue() - ( ( IndexCount * 2 ) + _IndexOffset );
            reader.BaseStream.Position = data.IndexBufferOffsets[lod] + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 + padding ) );
        }

        public abstract void Draw();
    }
}
