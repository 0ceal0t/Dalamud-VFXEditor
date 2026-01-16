using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow {
    public class MdlTerrainShadowSubmesh : MdlSubmeshData<MdlTerrainShadowMesh> {
        private readonly ParsedShort Unknown1 = new( "Unknown 1" );
        private readonly ParsedShort Unknown2 = new( "Unknown 2" );

        public MdlTerrainShadowSubmesh( MdlFile file, MdlTerrainShadowMesh parent ) : base( file, parent ) { }

        public MdlTerrainShadowSubmesh( MdlFile file, BinaryReader reader ) : base( file, null ) {
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

        public void PopulateWrite( MdlWriteData data ) {
            data.TerrainShadowSubmeshes.Add( this );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( _IndexOffset / 2 );
            writer.Write( IndexCount );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
        }
    }
}
