using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlExtraLod : IUiItem {
        public readonly MdlFile File;

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

        private readonly List<MdlMesh> LightShaftMeshes = [];
        private readonly UiDropdown<MdlMesh> LightShaftMeshView;

        private readonly List<MdlMesh> GlassMeshes = [];
        private readonly UiDropdown<MdlMesh> GlassMeshView;

        private readonly List<MdlMesh> MaterialChangeMeshes = [];
        private readonly UiDropdown<MdlMesh> MaterialChangeMeshView;

        private readonly List<MdlMesh> CrestChangeMeshes = [];
        private readonly UiDropdown<MdlMesh> CrestChangeMeshView;

        public MdlExtraLod( MdlFile file, BinaryReader reader ) {
            File = file;

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

            // ======== VIEWS ===========

            LightShaftMeshView = new( "Light Shaft Mesh", LightShaftMeshes );
            GlassMeshView = new( "Glass Mesh", GlassMeshes );
            MaterialChangeMeshView = new( "Material Change Mesh", MaterialChangeMeshes );
            CrestChangeMeshView = new( "Crest Change Mesh", CrestChangeMeshes );
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) DrawParameters();
            }

            using( var tab = ImRaii.TabItem( "Light Shafts" ) ) {
                if( tab ) LightShaftMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Glass" ) ) {
                if( tab ) GlassMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Material Change" ) ) {
                if( tab ) MaterialChangeMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Crest Change" ) ) {
                if( tab ) CrestChangeMeshView.Draw();
            }
        }

        public void DrawParameters() {
            using var child = ImRaii.Child( "Child" );

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

        public void Populate( MdlFileData data, BinaryReader reader, int lod ) {
            LightShaftMeshes.AddRange( data.Meshes.GetRange( _LightShaftMeshIndex, _LightShaftMeshCount ) );
            foreach( var mesh in LightShaftMeshes ) mesh.Populate( data, reader, lod );

            GlassMeshes.AddRange( data.Meshes.GetRange( _GlassMeshIndex, _GlassMeshCount ) );
            foreach( var mesh in GlassMeshes ) mesh.Populate( data, reader, lod );

            MaterialChangeMeshes.AddRange( data.Meshes.GetRange( _MaterialChangeMeshIndex, _MaterialChangeMeshCount ) );
            foreach( var mesh in MaterialChangeMeshes ) mesh.Populate( data, reader, lod );

            CrestChangeMeshes.AddRange( data.Meshes.GetRange( _CrestChangetMeshIndex, _CrestChangeMeshCount ) );
            foreach( var mesh in CrestChangeMeshes ) mesh.Populate( data, reader, lod );
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            foreach( var mesh in LightShaftMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in GlassMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in MaterialChangeMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in CrestChangeMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in LightShaftMeshes ) mesh.PopulateWrite( data, lod );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            data.WriteIndexCount( writer, LightShaftMeshes, _LightShaftMeshIndex );
            data.WriteIndexCount( writer, GlassMeshes, _GlassMeshIndex );
            data.WriteIndexCount( writer, MaterialChangeMeshes, _MaterialChangeMeshIndex );
            data.WriteIndexCount( writer, CrestChangeMeshes, _CrestChangetMeshIndex );

            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            Unknown9.Write( writer );
            Unknown10.Write( writer );
            Unknown11.Write( writer );
            Unknown12.Write( writer );
        }
    }
}
