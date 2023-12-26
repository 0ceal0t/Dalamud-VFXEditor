using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public MdlVertexDeclaration() { }

        public MdlVertexDeclaration( BinaryReader reader ) : this() {
            var endPos = reader.BaseStream.Position + ( 8 * 17 );

            Dalamud.Log( $"--------------" );

            for( var i = 0; i < 17; i++ ) {
                var element = new MdlVertexElement( reader );
                if( element.End ) break;
                Elements.Add( element );
            }

            Dalamud.Log( $"--------------" );

            reader.BaseStream.Position = endPos;
        }

        public List<int> GetStrides() {
            var streamElements = new List<List<MdlVertexElement>> { new(), new(), new() }; // which elements belong to which stream
            foreach( var element in Elements ) {
                if( element.End ) break;
                streamElements[element.Stream].Add( element );
            }

            var strides = new List<int>() { 0, 0, 0 };
            for( var i = 0; i < 3; i++ ) {
                if( streamElements[i].Count == 0 ) continue;
                var last = streamElements[i].Last();
                strides[i] = last.Offset + last.Size;
            }

            return strides;
        }
    }
}
