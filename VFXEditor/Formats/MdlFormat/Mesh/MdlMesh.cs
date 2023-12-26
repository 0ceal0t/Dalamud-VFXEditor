using SharpDX.Direct3D11;
using System.IO;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public struct DataRange {
        public uint Start;
        public uint End;
        public uint Count;
        public uint Stride;
    }

    public class MdlMesh : IUiItem {
        public readonly MdlVertexDeclaration Format;

        private ParsedString Material = new( "Material" );
        private ushort _MaterialStringIdx;
        private ushort _SubmeshIndex;
        private ushort _SubmeshCount;
        private ParsedShort BoneTableIndex = new( "Bone Table Index" );

        private ushort VertexCount; // Maxes out at ushort.MaxValue
        private uint IndexCount;

        private uint _IndexOffset;
        private uint[] _VertexBufferOffsets;

        private byte[] RawIndexData;
        private byte[] RawVertexData;
        private Buffer Data; // starts as null

        public MdlMesh() {
            Format = new();
        }

        public MdlMesh( MdlVertexDeclaration format, BinaryReader reader ) : this() {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            BoneTableIndex.Read( reader );
            _IndexOffset = 2 * reader.ReadUInt32();

            _VertexBufferOffsets = new[] { reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32() };
            reader.ReadBytes( 3 ); // strides
            reader.ReadByte(); // stream count

            // var vertexBufferStride = GetVertexBufferStride( vertexDeclarations[i] ).ConvertAll( x => ( byte )x ).ToArray();
            // var vertexBufferOffsets = new List<int>() { vertexBufferOffset, 0, 0 };
            // for( var j = 1; j < 3; j++ ) {
            //     if( vertexBufferStride[j - 1] > 0 ) { vertexBufferOffset += vertexBufferStride[j - 1] * vertexCount; }
            //     if( vertexBufferStride[j] > 0 ) { vertexBufferOffsets[j] = vertexBufferOffset; }
            // }

            // StartIndex = ( uint )indexData.Count / 2,
            // VertexBufferOffset = vertexBufferOffsets.ConvertAll( x => ( uint )x ).ToArray(),
            // VertexBufferStride = vertexBufferStride,
            // VertexStreamCount = ( byte )vertexBufferStride.Where( x => x > 0 ).Count()
        }

        public Buffer GetBuffer( Device device, out int count ) {
            if( Data == null ) RefreshBuffer( device );
            count = VertexCount;
            return Data;
        }

        public void RefreshBuffer( Device device ) {
            Data?.Dispose();
            // TODO
        }

        public void Populate( BinaryReader reader, uint vertexBufferPos, uint indexBufferPos ) {
            reader.BaseStream.Position = indexBufferPos + _IndexOffset;
            RawIndexData = reader.ReadBytes( ( int )( IndexCount * 2 ) );

            reader.BaseStream.Position = vertexBufferPos + _VertexBufferOffsets[0];
            var totalStride = Format.GetStride( 0 ) + Format.GetStride( 1 ) + Format.GetStride( 2 );
            RawVertexData = reader.ReadBytes( VertexCount * totalStride );
        }

        public void Draw() {
            Material.Draw();
            BoneTableIndex.Draw();
        }
    }
}
