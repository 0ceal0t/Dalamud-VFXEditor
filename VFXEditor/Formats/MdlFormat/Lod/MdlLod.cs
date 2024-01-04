using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Bone;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Formats.MdlFormat.Mesh.TerrainShadow;
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

        private readonly List<MdlMesh> Meshes = new();
        private readonly CommandDropdown<MdlMesh> MeshView;

        private readonly List<MdlTerrainShadowMesh> TerrainShadows = new();
        private readonly CommandDropdown<MdlTerrainShadowMesh> TerrainShadowView;

        private readonly List<MdlMesh> WaterMeshes = new();
        private readonly CommandDropdown<MdlMesh> WaterMeshView;

        private readonly List<MdlMesh> ShadowMeshes = new();
        private readonly CommandDropdown<MdlMesh> ShadowMeshView;

        private readonly List<MdlMesh> VerticalFogMeshes = new();
        private readonly CommandDropdown<MdlMesh> VerticalFogMeshView;

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
                Dalamud.Error( $"LoD: {edgeGeometrySize}/{edgeGeometryOffset} {polygonCount} {unknown}" );
            }

            MeshView = new( "Mesh", Meshes, null, () => new( File ) );
            TerrainShadowView = new( "Terrain Shadow", TerrainShadows, null, () => new( File ) );
            WaterMeshView = new( "Water Mesh", WaterMeshes, null, () => new( File ) );
            ShadowMeshView = new( "Shadow Mesh", ShadowMeshes, null, () => new( File ) );
            VerticalFogMeshView = new( "Vertical Fog Mesh", VerticalFogMeshes, null, () => new( File ) );
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

        public void Populate(
            List<MdlMesh> meshes,
            List<MdlTerrainShadowMesh> terrainShadows,
            List<MdlSubMesh> submeshes,
            List<MdlTerrainShadowSubmesh> terrainShadowSubmeshes,
            BinaryReader reader, uint vertexBufferPos, uint indexBufferPos,
            List<string> attributeStrings, List<string> materialStrings, List<string> boneStrings,
            List<MdlBoneTable> boneTables ) {

            Meshes.AddRange( meshes.GetRange( _MeshIndex, _MeshCount ) );
            foreach( var mesh in Meshes ) mesh.Populate( submeshes, reader, vertexBufferPos, indexBufferPos, materialStrings, boneStrings, boneTables );

            TerrainShadows.AddRange( terrainShadows.GetRange( _TerrainShadowMeshIndex, _TerrainShadowMeshCount ) );
            foreach( var mesh in TerrainShadows ) mesh.Populate( terrainShadowSubmeshes, reader, vertexBufferPos, indexBufferPos );

            WaterMeshes.AddRange( meshes.GetRange( _WaterMeshIndex, _WaterMeshCount ) );
            foreach( var mesh in WaterMeshes ) mesh.Populate( submeshes, reader, vertexBufferPos, indexBufferPos, materialStrings, boneStrings, boneTables );

            ShadowMeshes.AddRange( meshes.GetRange( _ShadowMeshIndex, _ShadowMeshCount ) );
            foreach( var mesh in ShadowMeshes ) mesh.Populate( submeshes, reader, vertexBufferPos, indexBufferPos, materialStrings, boneStrings, boneTables );

            VerticalFogMeshes.AddRange( meshes.GetRange( _VerticalFogMeshIndex, _VerticalFogMeshCount ) );
            foreach( var mesh in VerticalFogMeshes ) mesh.Populate( submeshes, reader, vertexBufferPos, indexBufferPos, materialStrings, boneStrings, boneTables );
        }
    }
}
