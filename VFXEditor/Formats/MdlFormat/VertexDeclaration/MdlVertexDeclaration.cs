using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MdlFormat.VertexDeclaration {
    public class MdlVertexDeclaration {
        public readonly List<MdlVertexElement> Elements = new();

        public MdlVertexDeclaration() { }

        public MdlVertexDeclaration( BinaryReader reader ) : this() {
            var endPos = reader.BaseStream.Position + ( 8 * 17 );

            for( var i = 0; i < 17; i++ ) {
                var element = new MdlVertexElement( reader );
                if( element.End ) break;
                Elements.Add( element );
            }

            reader.BaseStream.Position = endPos;
        }
    }
}
