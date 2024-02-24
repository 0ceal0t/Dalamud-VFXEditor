using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Formats.MdlFormat.Vertex {
    public enum VertexType : byte {
        Single3 = 2,
        Single4 = 3,
        UInt = 5,
        ByteFloat4 = 8,
        Half2 = 13,
        Half4 = 14
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
        public readonly List<MdlVertexElement> Elements = new();

        public MdlVertexDeclaration( BinaryReader reader ) {
            var endPos = reader.BaseStream.Position + ( 8 * 17 );

            for( var i = 0; i < 17; i++ ) {
                var element = new MdlVertexElement( reader );
                if( element.End ) break;
                Elements.Add( element );
            }

            reader.BaseStream.Position = endPos;
        }

        public void Write( BinaryWriter writer ) {
            foreach( var element in Elements ) element.Write( writer );
            writer.Write( ( byte )255 );
            for( var i = 0; i < 7 + 8 * ( 17 - Elements.Count - 1 ); i++ ) writer.Write( ( byte )0 );
        }

        public int GetStride( int stream ) {
            var elements = GetElements( stream );
            if( elements.Count == 0 ) return 0;

            return elements.Last().EndOffset;
        }

        private List<MdlVertexElement> GetElements( int stream ) => Elements.Where( x => x.Stream == stream ).ToList();

        public Vector4[] GetData( byte[] rawIndex, List<byte[]> vertexStreams, int indexCount, int vertexCount ) {
            var data = new List<Vector4>();

            var positions = new List<Vector4>();
            var tangents = new List<Vector4>();
            var uvs = new List<Vector4>();
            var normals = new List<Vector4>();
            var colors = new List<Vector4>();

            for( var i = 0; i < vertexStreams.Count; i++ ) {
                var streamData = vertexStreams[i];
                var elements = GetElements( i ).OrderBy( x => x.Offset ).ToList();
                if( elements.Count == 0 ) continue;

                using var ms = new MemoryStream( streamData );
                using var reader = new BinaryReader( ms );

                for( var j = 0; j < vertexCount; j++ ) {
                    foreach( var element in elements ) {
                        var item = element.Read( reader );

                        if( element.Usage == VertexUsage.Position ) positions.Add( item );
                        else if( element.Usage == VertexUsage.Tangent1 ) tangents.Add( item );
                        else if( element.Usage == VertexUsage.UV ) uvs.Add( item );
                        else if( element.Usage == VertexUsage.Normal ) normals.Add( item );
                        else if( element.Usage == VertexUsage.Color ) colors.Add( item / 255f );
                    }
                }
            }

            // ====== INDEX ===========

            using var iMs = new MemoryStream( rawIndex );
            using var iReader = new BinaryReader( iMs );

            var a = new HashSet<short>();

            for( var i = 0; i < indexCount; i++ ) {
                var index = iReader.ReadInt16();

                data.Add( positions.Count == 0 ? new( 0 ) : positions[index] );
                data.Add( tangents.Count == 0 ? new( 0 ) : tangents[index] );
                data.Add( uvs.Count == 0 ? new( 0 ) : uvs[index] );
                data.Add( normals.Count == 0 ? new( 0 ) : normals[index] );
                data.Add( colors.Count == 0 ? new( 0 ) : colors[index] );
            }

            return data.ToArray();
        }
    }
}
