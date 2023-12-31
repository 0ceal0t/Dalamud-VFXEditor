using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public class MdlSubMesh : MdlMeshDrawable, IUiItem {
        private MdlMesh Parent;

        private readonly uint _IndexOffset;
        private readonly uint _AttributeIndexMask;
        private readonly ushort _BoneStartIndex;
        private readonly ushort _BoneCount;

        private readonly uint IndexCount;

        private byte[] RawIndexData;

        public MdlSubMesh( MdlMesh parent ) {
            Parent = parent;
            // TODO
        }

        public MdlSubMesh( BinaryReader reader ) {
            _IndexOffset = 2 * reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            _AttributeIndexMask = reader.ReadUInt32();
            _BoneStartIndex = reader.ReadUInt16();
            _BoneCount = reader.ReadUInt16();
        }

        public void Populate( MdlMesh mesh, BinaryReader reader, uint indexBufferPos ) {
            Parent = mesh;
            reader.BaseStream.Position = indexBufferPos + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 ) );
        }

        public override uint GetIndexCount() => Parent.GetIndexCount();

        public override void RefreshBuffer( Device device ) {
            if( Parent == null ) return;

            Data?.Dispose();
            var data = Parent.GetData( ( int )IndexCount, RawIndexData );
            Data = Buffer.Create( device, BindFlags.VertexBuffer, data );
        }

        public void Draw() {
            if( Parent == null ) return;

            if( Plugin.DirectXManager.MeshPreview.CurrentMesh != this ) {
                Plugin.DirectXManager.MeshPreview.LoadMesh( Parent.File, this );
            }
            Plugin.DirectXManager.MeshPreview.DrawInline();
        }
    }
}
