using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.MdlFormat.Lod {
    public class MdlLod : IUiItem {
        private readonly ParsedShort MeshIndex = new( "Mesh Index" );
        private readonly ParsedShort MeshCount = new( "Mesh Count" );
        private readonly ParsedFloat ModelRange = new( "Model Range" );
        private readonly ParsedFloat TextureRange = new( "Texture Range" );
        private readonly ParsedShort WaterMeshIndex = new( "Water Mesh Index" );
        private readonly ParsedShort WaterMeshCount = new( "Water Mesh Count" );
        private readonly ParsedShort ShadowMeshIndex = new( "Shaodw Mesh Index" );
        private readonly ParsedShort ShadowMeshCount = new( "Shaodw Mesh Count" );
        private readonly ParsedShort TerrainShadowMeshIndex = new( "Terrain Shadow Mesh Index" );
        private readonly ParsedShort TerrainShadowMeshCount = new( "Terrain Shadow Mesh Count" );
        private readonly ParsedShort VerticalFogMeshIndex = new( "Vertical Fog Mesh Index" );
        private readonly ParsedShort VerticalFogMeshCount = new( "Vertical Fog Mesh Count" );

        private uint EdgeGeometrySize;
        private uint EdgeGeometryOffset;
        private uint PolygonCount; // 0
        private uint Unknown; // 0
        private uint VertexBufferSize;
        private uint IndexBufferSize;
        private uint VertexDataOffset;
        private uint IndexDataOffset;

        public MdlLod( BinaryReader reader ) {
            MeshIndex.Read( reader );
            MeshCount.Read( reader );
            ModelRange.Read( reader );
            TextureRange.Read( reader );
            WaterMeshIndex.Read( reader );
            WaterMeshCount.Read( reader );
            ShadowMeshIndex.Read( reader );
            ShadowMeshCount.Read( reader );
            TerrainShadowMeshIndex.Read( reader );
            TerrainShadowMeshCount.Read( reader );
            VerticalFogMeshIndex.Read( reader );
            VerticalFogMeshCount.Read( reader );

            EdgeGeometrySize = reader.ReadUInt32();
            EdgeGeometryOffset = reader.ReadUInt32();
            PolygonCount = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            VertexBufferSize = reader.ReadUInt32();
            IndexBufferSize = reader.ReadUInt32();
            VertexDataOffset = reader.ReadUInt32();
            IndexDataOffset = reader.ReadUInt32();

            // https://github.com/xivdev/Xande/blob/8fc75ce5192edcdabc4d55ac93ca0199eee18bc9/Xande.GltfImporter/MdlFileBuilder.cs#L558
            Dalamud.Log( $"Lod: edge:{EdgeGeometrySize:X4}/{EdgeGeometryOffset:X4} {PolygonCount} {Unknown} size:{VertexBufferSize:X4}/{IndexBufferSize:X4} offset:{VertexDataOffset:X4}/{IndexDataOffset:X4}" );
        }

        public void Draw() {
            MeshIndex.Draw();
            MeshCount.Draw();
            ModelRange.Draw();
            TextureRange.Draw();
            WaterMeshIndex.Draw();
            WaterMeshCount.Draw();
            ShadowMeshIndex.Draw();
            ShadowMeshCount.Draw();
            TerrainShadowMeshIndex.Draw();
            TerrainShadowMeshCount.Draw();
            VerticalFogMeshIndex.Draw();
            VerticalFogMeshCount.Draw();
        }
    }
}
