using System.Collections.Generic;
using System.IO;

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

        public int GetStride( int stream ) {
            MdlVertexElement lastElement = null;
            foreach( var element in Elements ) {
                if( element.End ) break;
                if( element.Stream != stream ) continue;

                lastElement = element;
            }

            if( lastElement == null ) return 0;

            return lastElement.Offset + lastElement.Size;
        }

        /*
         * 2023-12-25 22:54:15.966 -05:00 [INF] [VFXEditor] Element: 0 0000 Half4 Position
2023-12-25 22:54:15.966 -05:00 [INF] [VFXEditor] Element: 1 0000 Half4 Normal
2023-12-25 22:54:15.966 -05:00 [INF] [VFXEditor] Element: 1 0008 ByteFloat4 Color
2023-12-25 22:54:15.966 -05:00 [INF] [VFXEditor] Element: 1 000C Half2 UV

        2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 0 0000 Half4 Position
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 0 0008 ByteFloat4 BlendWeights
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 0 000C UInt BlendIndices
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 1 0000 Half4 Normal
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 1 0008 ByteFloat4 Tangent1
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 1 000C ByteFloat4 Color
2023-12-25 22:37:25.419 -05:00 [INF] [VFXEditor] Element: 1 0010 Half4 UV
         */
    }
}
