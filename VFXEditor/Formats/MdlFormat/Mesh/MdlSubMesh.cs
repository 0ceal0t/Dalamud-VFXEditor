using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh.Base;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public class MdlSubMesh : MdlSubmeshData<MdlMesh> {
        private readonly uint _AttributeIndexMask;
        private readonly ushort _BoneStartIndex;
        private readonly ushort _BoneCount;

        public MdlSubMesh( MdlMesh parent ) : base( parent ) { }

        public MdlSubMesh( BinaryReader reader ) : base( null ) {
            _IndexOffset = 2 * reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            _AttributeIndexMask = reader.ReadUInt32();
            _BoneStartIndex = reader.ReadUInt16();
            _BoneCount = reader.ReadUInt16();
        }

        public override void Draw() => DrawPreview();
    }
}
