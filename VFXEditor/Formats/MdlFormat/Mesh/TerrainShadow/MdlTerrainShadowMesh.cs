using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow {
    public class MdlTerrainShadowMesh : MdlMeshDrawable, IUiItem {
        public readonly MdlFile File;

        private readonly ushort _SubmeshIndex;
        private readonly ushort _SubmeshCount;
        private readonly uint _IndexOffset;
        private readonly uint _VertexBufferOffset;

        private ushort VertexCount;
        private uint IndexCount;
        private byte Stride;

        // TODO: creating new
        public MdlTerrainShadowMesh( MdlFile file ) {
            File = file;
        }

        public MdlTerrainShadowMesh( MdlFile file, BinaryReader reader ) : this( file ) {
            IndexCount = reader.ReadUInt32();
            _IndexOffset = 2 * reader.ReadUInt32();
            _VertexBufferOffset = reader.ReadUInt32();
            VertexCount = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            Stride = reader.ReadByte();
            reader.ReadByte(); // padding
        }

        public void Draw() {
            // TODO
        }

        public override uint GetIndexCount() => IndexCount;

        public override void RefreshBuffer( Device device ) {
            // TODO
        }
    }
}
