using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlExtraLod : IUiItem {
        private readonly ParsedShort LightShaftMeshIndex = new( "Light Shaft Mesh Index" );
        private readonly ParsedShort LightShaftMeshCount = new( "Light Shaft Mesh Count" );
        private readonly ParsedShort GlassMeshIndex = new( "Glast Mesh Index" );
        private readonly ParsedShort GlassMeshCount = new( "Glass Mesh Count" );
        private readonly ParsedShort MaterialChangeMeshIndex = new( "Material Change Mesh Index" );
        private readonly ParsedShort MaterialChangeMeshCount = new( "MaterialChange Mesh Count" );
        private readonly ParsedShort CrestChangetMeshIndex = new( "Crest Change Mesh Index" );
        private readonly ParsedShort CrestChangeMeshCount = new( "Crest Change Mesh Count" );
        private readonly ParsedShort Unknown1 = new( "Unknown 1" );
        private readonly ParsedShort Unknown2 = new( "Unknown 2" );
        private readonly ParsedShort Unknown3 = new( "Unknown 3" );
        private readonly ParsedShort Unknown4 = new( "Unknown 4" );
        private readonly ParsedShort Unknown5 = new( "Unknown 5" );
        private readonly ParsedShort Unknown6 = new( "Unknown 6" );
        private readonly ParsedShort Unknown7 = new( "Unknown 7" );
        private readonly ParsedShort Unknown8 = new( "Unknown 8" );
        private readonly ParsedShort Unknown9 = new( "Unknown 9" );
        private readonly ParsedShort Unknown10 = new( "Unknown 10" );
        private readonly ParsedShort Unknown11 = new( "Unknown 11" );
        private readonly ParsedShort Unknown12 = new( "Unknown 12" );

        public MdlExtraLod() { }

        public MdlExtraLod( BinaryReader reader ) : this() {
            LightShaftMeshIndex.Read( reader );
            LightShaftMeshCount.Read( reader );
            GlassMeshIndex.Read( reader );
            GlassMeshCount.Read( reader );
            MaterialChangeMeshIndex.Read( reader );
            MaterialChangeMeshCount.Read( reader );
            CrestChangetMeshIndex.Read( reader );
            CrestChangeMeshCount.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            Unknown9.Read( reader );
            Unknown10.Read( reader );
            Unknown11.Read( reader );
            Unknown12.Read( reader );
        }

        public void Draw() {
            LightShaftMeshIndex.Draw();
            LightShaftMeshCount.Draw();
            GlassMeshIndex.Draw();
            GlassMeshCount.Draw();
            MaterialChangeMeshIndex.Draw();
            MaterialChangeMeshCount.Draw();
            CrestChangetMeshIndex.Draw();
            CrestChangeMeshCount.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
            Unknown9.Draw();
            Unknown10.Draw();
            Unknown11.Draw();
            Unknown12.Draw();
        }
    }
}
