using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlExtraLod : IUiItem {
        private readonly ushort _LightShaftMeshIndex;
        private readonly ushort _LightShaftMeshCount;
        private readonly ushort _GlassMeshIndex;
        private readonly ushort _GlassMeshCount;
        private readonly ushort _MaterialChangeMeshIndex;
        private readonly ushort _MaterialChangeMeshCount;
        private readonly ushort _CrestChangetMeshIndex;
        private readonly ushort _CrestChangeMeshCount;

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
            _LightShaftMeshIndex = reader.ReadUInt16();
            _LightShaftMeshCount = reader.ReadUInt16();
            _GlassMeshIndex = reader.ReadUInt16();
            _GlassMeshCount = reader.ReadUInt16();
            _MaterialChangeMeshIndex = reader.ReadUInt16();
            _MaterialChangeMeshCount = reader.ReadUInt16();
            _CrestChangetMeshIndex = reader.ReadUInt16();
            _CrestChangeMeshCount = reader.ReadUInt16();

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
