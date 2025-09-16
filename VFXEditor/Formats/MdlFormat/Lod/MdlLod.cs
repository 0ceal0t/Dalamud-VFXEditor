using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Mesh.Base;
using VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow;
using VfxEditor.Formats.MdlFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlLod : IUiItem {
        public readonly MdlFile File;

        private readonly ParsedFloat ModelRange = new( "Model Range" );
        private readonly ParsedFloat TextureRange = new( "Texture Range" );

        private readonly ushort _MeshIndex;
        private readonly ushort _MeshCount;
        private readonly ushort _TerrainShadowMeshIndex;
        private readonly ushort _TerrainShadowMeshCount;

        // Just regular meshes
        private readonly ushort _WaterMeshIndex;
        private readonly ushort _WaterMeshCount;
        private readonly ushort _ShadowMeshIndex;
        private readonly ushort _ShadowMeshCount;
        private readonly ushort _VerticalFogMeshIndex;
        private readonly ushort _VerticalFogMeshCount;

        private readonly List<MdlMesh> Meshes = [];
        private readonly UiDropdown<MdlMesh> MeshView;

        private readonly List<MdlTerrainShadowMesh> TerrainShadows = [];
        private readonly UiDropdown<MdlTerrainShadowMesh> TerrainShadowView;

        private readonly List<MdlMesh> WaterMeshes = [];
        private readonly UiDropdown<MdlMesh> WaterMeshView;

        private readonly List<MdlMesh> ShadowMeshes = [];
        private readonly UiDropdown<MdlMesh> ShadowMeshView;

        private readonly List<MdlMesh> VerticalFogMeshes = [];
        private readonly UiDropdown<MdlMesh> VerticalFogMeshView;

        public MdlLod( MdlFile file, BinaryReader reader ) {
            File = file;

            _MeshIndex = reader.ReadUInt16();
            _MeshCount = reader.ReadUInt16();
            ModelRange.Read( reader );
            TextureRange.Read( reader );
            _WaterMeshIndex = reader.ReadUInt16();
            _WaterMeshCount = reader.ReadUInt16();
            _ShadowMeshIndex = reader.ReadUInt16();
            _ShadowMeshCount = reader.ReadUInt16();
            _TerrainShadowMeshIndex = reader.ReadUInt16();
            _TerrainShadowMeshCount = reader.ReadUInt16();
            _VerticalFogMeshIndex = reader.ReadUInt16();
            _VerticalFogMeshCount = reader.ReadUInt16();

            var edgeGeometrySize = reader.ReadUInt32();
            var edgeGeometryOffset = reader.ReadUInt32(); // equal to `vertexBufferOffset + vertexBufferSize` if `edgeGeometrySize = 0`
            var polygonCount = reader.ReadUInt32();
            var unknown = reader.ReadUInt32();

            reader.ReadUInt32(); // vertex buffer size, same as MdlFile
            reader.ReadUInt32(); // index buffer size
            reader.ReadUInt32(); // vertex data offset
            reader.ReadUInt32(); // index data offset

            // https://github.com/xivdev/Xande/blob/8fc75ce5192edcdabc4d55ac93ca0199eee18bc9/Xande.GltfImporter/MdlFileBuilder.cs#L558
            if( edgeGeometrySize != 0 || polygonCount != 0 || unknown != 0 ) {
                Dalamud.Error( $"LoD: {edgeGeometrySize:X4} {edgeGeometryOffset:X4} {polygonCount:X4} {unknown:X4}" );
            }

            // ========= VIEWS ==============

            MeshView = new( "Mesh", Meshes );
            TerrainShadowView = new( "Terrain Shadow", TerrainShadows );
            WaterMeshView = new( "Water Mesh", WaterMeshes );
            ShadowMeshView = new( "Shadow Mesh", ShadowMeshes );
            VerticalFogMeshView = new( "Vertical Fog Mesh", VerticalFogMeshes );
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Meshes" ) ) {
                if( tab ) MeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Terrain Shadows" ) ) {
                if( tab ) TerrainShadowView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Water" ) ) {
                if( tab ) WaterMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Shadows" ) ) {
                if( tab ) ShadowMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Vertical Fog" ) ) {
                if( tab ) VerticalFogMeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) {
                    ModelRange.Draw();
                    TextureRange.Draw();
                }
            }
        }

        public void Populate( MdlFileData data, BinaryReader reader, int lod ) {
            if( _MeshIndex < data.Meshes.Count )
                Meshes.AddRange( data.Meshes.GetRange( _MeshIndex, _MeshCount ) );

            if( _TerrainShadowMeshIndex < data.Meshes.Count )
                TerrainShadows.AddRange( data.TerrainShadowMeshes.GetRange( _TerrainShadowMeshIndex, _TerrainShadowMeshCount ) );

            if( _WaterMeshIndex < data.Meshes.Count )
                WaterMeshes.AddRange( data.Meshes.GetRange( _WaterMeshIndex, _WaterMeshCount ) );

            if( _ShadowMeshIndex < data.Meshes.Count )
                ShadowMeshes.AddRange( data.Meshes.GetRange( _ShadowMeshIndex, _ShadowMeshCount ) );

            if( _VerticalFogMeshIndex < data.Meshes.Count )
                VerticalFogMeshes.AddRange( data.Meshes.GetRange( _VerticalFogMeshIndex, _VerticalFogMeshCount ) );

            var allMeshes = new List<MdlMeshDrawable>();
            allMeshes.AddRange( Meshes );
            allMeshes.AddRange( WaterMeshes );
            allMeshes.AddRange( ShadowMeshes );
            allMeshes.AddRange( TerrainShadows );
            allMeshes.AddRange( VerticalFogMeshes );

            foreach( var mesh in allMeshes ) {
                var indexOffset = mesh.IndexBufferOffset;
                if( indexOffset == 0 ) continue;
                data.IndexBufferPositions[lod].Enqueue( indexOffset );
            }
            data.IndexBufferPositions[lod].Enqueue( data.IndexBufferSizes[lod] );

            foreach( var mesh in Meshes ) mesh.Populate( data, reader, lod );
            foreach( var mesh in TerrainShadows ) mesh.Populate( data, reader, lod );
            foreach( var mesh in WaterMeshes ) mesh.Populate( data, reader, lod );
            foreach( var mesh in ShadowMeshes ) mesh.Populate( data, reader, lod );
            foreach( var mesh in VerticalFogMeshes ) mesh.Populate( data, reader, lod );
        }

        public void PopulateWrite( MdlWriteData data, int lod ) {
            foreach( var mesh in Meshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in TerrainShadows ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in WaterMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in ShadowMeshes ) mesh.PopulateWrite( data, lod );
            foreach( var mesh in VerticalFogMeshes ) mesh.PopulateWrite( data, lod );
        }

        public void Write( BinaryWriter writer, MdlWriteData data ) {
            data.WriteIndexCount( writer, Meshes, _MeshIndex );
            ModelRange.Write( writer );
            TextureRange.Write( writer );
            data.WriteIndexCount( writer, WaterMeshes, _WaterMeshIndex );
            data.WriteIndexCount( writer, ShadowMeshes, _ShadowMeshIndex );
            data.WriteIndexCount( writer, TerrainShadows, _TerrainShadowMeshIndex );
            data.WriteIndexCount( writer, VerticalFogMeshes, _VerticalFogMeshIndex );

            data.LodPlaceholders[this] = writer.BaseStream.Position; // Placeholders

            writer.Write( 0 ); // Edge Geometry size
            writer.Write( 0 ); // Edge Geometry offset
            writer.Write( 0 ); // Polygon count
            writer.Write( 0 ); // Unknown

            writer.Write( 0 ); // Vertex buffer size
            writer.Write( 0 ); // Index buffer size
            writer.Write( 0 ); // Vertex data offset
            writer.Write( 0 ); // Index data offset
        }
    }
}
