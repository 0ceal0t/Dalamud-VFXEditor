using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MdlFormat.Mesh;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlLod : IUiItem {
        public readonly MdlFile File;

        // TOOD: should mesh count equal # of mesh objects?
        private readonly ushort _MeshIndex;
        private readonly ushort _MeshCount;
        private readonly ParsedFloat ModelRange = new( "Model Range" );
        private readonly ParsedFloat TextureRange = new( "Texture Range" );
        private readonly ushort _WaterMeshIndex;
        private readonly ushort _WaterMeshCount;
        private readonly ushort _ShadowMeshIndex;
        private readonly ushort _ShadowMeshCount;
        private readonly ushort _TerrainShadowMeshIndex;
        private readonly ushort _TerrainShadowMeshCount;
        private readonly ushort _VerticalFogMeshIndex;
        private readonly ushort _VerticalFogMeshCount;

        private uint EdgeGeometrySize;
        private uint EdgeGeometryOffset;
        private uint PolygonCount; // 0
        private uint Unknown; // 0

        private readonly List<MdlMesh> Meshes = new();
        private readonly CommandDropdown<MdlMesh> MeshView;

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

            EdgeGeometrySize = reader.ReadUInt32();
            EdgeGeometryOffset = reader.ReadUInt32();
            PolygonCount = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            reader.ReadUInt32(); // vertex buffer size, same as MdlFile
            reader.ReadUInt32(); // index buffer size
            reader.ReadUInt32(); // vertex data offset
            reader.ReadUInt32(); // index data offset

            // https://github.com/xivdev/Xande/blob/8fc75ce5192edcdabc4d55ac93ca0199eee18bc9/Xande.GltfImporter/MdlFileBuilder.cs#L558
            Dalamud.Log( $"Lod: edge:{EdgeGeometrySize:X4}/{EdgeGeometryOffset:X4} {PolygonCount} {Unknown}" );

            MeshView = new( "Mesh", Meshes, null, () => new( File ) );
        }

        public void Draw() {
            using var tabBar = ImRaii.TabBar( "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton );
            if( !tabBar ) return;

            using( var tab = ImRaii.TabItem( "Meshes" ) ) {
                if( tab ) MeshView.Draw();
            }

            using( var tab = ImRaii.TabItem( "Parameters" ) ) {
                if( tab ) {
                    ModelRange.Draw();
                    TextureRange.Draw();
                }
            }
        }

        public void Populate( List<MdlMesh> meshes, BinaryReader reader, uint vertexBufferPos, uint indexBufferPos ) {
            Meshes.AddRange( meshes.GetRange( _MeshIndex, _MeshCount ) );
            foreach( var mesh in Meshes ) mesh.Populate( reader, vertexBufferPos, indexBufferPos );
        }
    }
}
