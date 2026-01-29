using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Formats.MdlFormat.Vertex {
    public enum VertexType : byte {
        Single3 = 2,
        Single4 = 3,
        UInt = 5,
        Short2 = 6,
        Short4 = 7,
        ByteFloat4 = 8,
        Short2n = 9,
        Short4n = 10,
        Half2 = 13,
        Half4 = 14,
        UShort2 = 16,
        UShort4 = 17
    }

    public enum VertexUsage : byte {
        Position = 0,
        BlendWeights = 1,
        BlendIndices = 2,
        Normal = 3,
        UV = 4,
        Tangent2 = 5,
        Tangent1 = 6,
        Color = 7,
    }

    // https://github.com/xivdev/Xande/blob/8fc75ce5192edcdabc4d55ac93ca0199eee18bc9/Xande.GltfImporter/MdlFileBuilder.cs#L127

    public class MdlVertexDeclaration {
        public readonly List<MdlVertexElement> Elements = [];

        public MdlVertexDeclaration( BinaryReader reader ) {
            for( var i = 0; i < 17; i++ ) {
                Elements.Add( new MdlVertexElement( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            foreach( var element in Elements ) element.Write( writer );
        }

        private List<MdlVertexElement> GetElements( int stream ) => [.. Elements.Where( x => !x.NoData && x.Stream == stream )];

        public Vector4[] GetData( byte[] rawIndex, List<byte[]> vertexStreams, int indexCount, int vertexCount, byte[] strides ) {
            var data = new List<Vector4>();

            var positions = new List<Vector4>();
            var tangents = new List<Vector4>();
            var uvs = new List<Vector4>();
            var normals = new List<Vector4>();
            var colors = new List<Vector4>();

            for( var i = 0; i < vertexStreams.Count; i++ ) {
                var streamData = vertexStreams[i];
                var stride = strides[i];

                var elements = GetElements( i ).OrderBy( x => x.Offset ).ToList();
                if( elements.Count == 0 ) continue;

                using var ms = new MemoryStream( streamData );
                using var reader = new BinaryReader( ms );

                for( var j = 0; j < vertexCount; j++ ) {
                    var startPos = reader.BaseStream.Position;
                    var colorFound = false;
                    foreach( var element in elements ) {
                        var item = element.Read( reader );

                        if( element.Usage == VertexUsage.Position ) positions.Add( item );
                        else if( element.Usage == VertexUsage.Tangent1 ) tangents.Add( Vector4.Normalize( item - new Vector4( 0.5f, 0.5f, 0.5f, 0f ) ) );
                        else if( element.Usage == VertexUsage.UV ) uvs.Add( item );
                        else if( element.Usage == VertexUsage.Normal ) normals.Add( item );
                        else if( element.Usage == VertexUsage.Color && !colorFound ) {
                            colors.Add( item );
                            colorFound = true;
                        }
                    }
                    reader.BaseStream.Position = startPos + stride;
                }
            }

            // ====== INDEX ===========

            using var iMs = new MemoryStream( rawIndex );
            using var iReader = new BinaryReader( iMs );

            for( var i = 0; i < indexCount; i++ ) {
                var index = iReader.ReadInt16();

                data.Add( positions.Count == 0 ? new( 0 ) : positions[index] );
                data.Add( tangents.Count == 0 ? new( 0 ) : tangents[index] );
                data.Add( uvs.Count == 0 ? new( 0 ) : uvs[index] );
                data.Add( normals.Count == 0 ? new( 0 ) : normals[index] );
                data.Add( colors.Count == 0 ? new( 0 ) : colors[index] );
            }

            return [.. data];
        }
    }
}
