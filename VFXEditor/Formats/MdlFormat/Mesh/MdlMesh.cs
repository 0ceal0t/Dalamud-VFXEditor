using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Vertex;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Mesh {
    public struct DataRange {
        public uint Stream;
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
        private List<DataRange> VertexRange = new();
        private DataRange IndexRange;

        public MdlMesh() {
            Format = new();
        }

        public MdlMesh( int idx, int meshCount, MdlVertexDeclaration format, BinaryReader reader ) : this() {
            Format = format;

            VertexCount = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
            IndexCount = reader.ReadUInt32();
            _MaterialStringIdx = reader.ReadUInt16();
            _SubmeshIndex = reader.ReadUInt16();
            _SubmeshCount = reader.ReadUInt16();
            BoneTableIndex.Read( reader );
            var startIndex = reader.ReadUInt32();

            var vertexBufferOffset = new[] { reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32() };
            var vertexBufferStride = reader.ReadBytes( 3 );
            var vertexStreamCount = reader.ReadByte();

            var meshesPerIndexBuffer = Math.Ceiling( meshCount / 3f );
            var indexBuffer = ( int )Math.Floor( idx / meshesPerIndexBuffer );

            for( var i = 0u; i < vertexStreamCount; i++ ) {
                VertexRange.Add( new() {
                    Stream = i,
                    Start = vertexBufferOffset[i],
                    End = ( uint )( vertexBufferOffset[i] + ( vertexBufferStride[i] * VertexCount ) ),
                    Count = VertexCount,
                    Stride = vertexBufferStride[i]
                } );
            }

            IndexRange = new() {
                Stream = ( uint )indexBuffer,
                Start = startIndex * 2,
                End = ( startIndex + IndexCount ) * 2,
                Count = IndexCount,
                Stride = 2
            };

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

        public void Draw() {
            Material.Draw();
            BoneTableIndex.Draw();
        }
    }
}
