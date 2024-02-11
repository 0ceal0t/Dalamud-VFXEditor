using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow {
    public class MdlTerrainShadowSubmesh : MdlSubmeshData<MdlTerrainShadowMesh> {
        private readonly ParsedShort Unknown1 = new( "Unknown 1" );
        private readonly ParsedShort Unknown2 = new( "Unknown 2" );

        public MdlTerrainShadowSubmesh( MdlTerrainShadowMesh parent ) : base( parent ) { }

        public MdlTerrainShadowSubmesh( BinaryReader reader ) : base( null ) {
            _IndexOffset = 2 * reader.ReadUInt32();
            IndexCount = reader.ReadUInt32();
            Unknown1.Read( reader );
            Unknown2.Read( reader );
        }

        public override void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            DrawPreview();
        }
    }
}
